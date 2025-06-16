import { useEffect, useState ,useRef,useMemo} from "react";
import './WorkHourViewer.css';
function WorkHourViewer() {
  const maxChar_des=30;
  const [staffList, setStaffList] = useState([]);
  const [selectedStaffId, setSelectedStaffId] = useState(null);
  // const [selectedStaffName,selSelectedStaffName]=useState(null);
  const selectedStaffName = useMemo(() => {
  return staffList.find(s => s.staffId === selectedStaffId)?.name || "";
}, [selectedStaffId, staffList]);


  const [selectedDate, setSelectedDate] = useState(new Date());
  const [workHours, setWorkHours] = useState([]);
  const [JobList,setJobList]=useState([]);
  const [selectedJobId, setSelectedJobId]=useState(null);
  const selectedJobName = useMemo(() => {
  return JobList.find(s => s.jobId === selectedJobId)?.name || "";
}, [selectedStaffId, staffList]);


  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [taskDescription, setTaskDescription] = useState("");

  const refreshWorkHours = () => {
    

    const isoStart = weekDates[0].toISOString().split("T")[0];
    const isoEnd = weekDates[6].toISOString().split("T")[0];

    fetch(`${apiUrl}/WorkHour/get_workhours_byrange/${selectedStaffId}?startDate=${isoStart}&endDate=${isoEnd}`)
      .then((res) => res.json())
      .then(setWorkHours)
      .catch(console.error);
  };

  const apiUrl = import.meta.env.VITE_API_URL;



  const getWeekRange = (date) => {
  const monday = new Date(date);
  monday.setDate(date.getDate() - ((date.getDay() + 6) % 7));
  const week = Array.from({ length: 7 }).map((_, i) => {
    const d = new Date(monday);
    d.setDate(monday.getDate() + i);
    return d;
  });
  return week;
};

  const weekDates = getWeekRange(selectedDate);

// To get days according to start date and end date by user
  const getDateRange = (start, end) => {
  const result = [];
  const current = new Date(start);
  const last = new Date(end);
  while (current <= last) {
    result.push(new Date(current));
    current.setDate(current.getDate() + 1);
  }
  return result;
};

//Event to Set work Hours
const handleSetWorkTime = async () => {
  console.log("selectedStaffId:", selectedStaffId);
console.log("selectedJobId:", selectedJobId);
console.log("startDate:", startDate);
console.log("endDate:", endDate);
console.log("startTime:", startTime);
console.log("endTime:", endTime);
  if (!selectedStaffId || !selectedJobId || !startDate || !endDate || !startTime || !endTime) 
  { 
    alert("Please fill all fields");
    return;
  }

  const dateRange = getDateRange(startDate, endDate);
  console.log("dataRange:"+dateRange);
  const workHourDtos = dateRange.map(date => ({
  staffId: selectedStaffId,
  jobId: selectedJobId,
  onWork: true,
  date: date.toISOString().split("T")[0],
  startTime: `${startTime}:00`,
  endTime: `${endTime}:00`,
  taskDescription: taskDescription,
}));
console.log(workHourDtos);
  try {
    
    const response = await fetch(`${apiUrl}/WorkHour/hours_set_byStaff/${selectedStaffId}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(workHourDtos),
    });

    if (response.ok) {
      alert("Work hours set successfully!");
      console.log('try to refresh after set');
      if (!selectedStaffId) {
      
      alert('No selected staff')
      return;
      }
       // Refresh work hours
      refreshWorkHours();
    } 
    else {
      const error = await response.text();
      alert(`Error: ${error}`);
    }
  } catch (err) {
    alert("Network error: " + err.message);
  }
};

const handleCancelWorkTime = async () => {
  if (!selectedStaffId ||  !startDate || !endDate || !startTime || !endTime) {
    alert("Please fill all fields to cancel.");
    return;
  }

  const dateRange = getDateRange(startDate, endDate);
  const deleteDtos = dateRange.map(date => ({
    staffId: selectedStaffId,
    jobId: selectedJobId,
    onWork: true,
    date: date.toISOString().split("T")[0],
    startTime: `${startTime}:00`,
    endTime: `${endTime}:00`,
    taskDescription: taskDescription || "", // Optional
  }));

  try {
    const response = await fetch(`${apiUrl}/WorkHour/hours_delete_by_fields`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(deleteDtos),
    });

    if (response.ok) {
      alert("Work time cancelled successfully.");
      refreshWorkHours();
    } else {
      const err = await response.text();
      alert("Failed to cancel work time: " + err);
    }
  } catch (err) {
    alert("Network error: " + err.message);
  }
};

const handleQueryOvertime = async () => {
  if (!selectedStaffId || !startDate || !endDate) {
    alert("Please select staff, start date, and end date.");
    return;
  }

  try {
    const response = await fetch(
      `${apiUrl}/WorkHour/get_overtime_days/${selectedStaffId}?startDate=${startDate}&endDate=${endDate}`
    );

    if (response.ok) {
      const result = await response.json(); // e.g., ["2025-06-10", "2025-06-12"]
      alert("overworkDays: "+result);
    } else {
      const err = await response.text();
      alert("Failed to fetch overtime days: " + err);
    }
  } catch (err) {
    alert("Network error: " + err.message);
  }
};

const handleQueryRemainingMinutes = async () => {
  console.log(selectedStaffId);
  console.log(selectedJobId);
  console.log(endDate);
  console.log(endTime);
  if (!selectedStaffId || !selectedJobId || !endTime || !endDate) {
    alert("Please select staff, job, end time, and end date.");
    return;
  }

  try {
    const response = await fetch(
      `${apiUrl}/WorkHour/remaining_worktime?staffId=${selectedStaffId}&jobId=${selectedJobId}&endDate=${endDate}&endTime=${endTime}`
    );

    if (response.ok) {
      const result = await response.json(); // a number
      const roundedMinutes = Math.round(result.remainingMinutes);
      const hours = Math.floor(roundedMinutes / 60);
      const minutes = roundedMinutes % 60;
      const timeFormatted = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
      alert(`Remaining work time for ${selectedStaffName} at ${selectedJobName} until ${endTime} ${endDate} is ${timeFormatted}`);
    } else {
      const err = await response.text();
      alert("Failed to fetch remaining minutes: " + err);
    }
  } catch (err) {
    alert("Network error: " + err.message);
  }
};

  // Fetch all staff on mount
  useEffect(() => {
    fetch(`${apiUrl}/staff`)
      .then((res) => res.json())
      .then(setStaffList)
      .catch(console.error);
  }, []);

  //Todo: get all jobs list
    useEffect(()=>{
      fetch(`${apiUrl}/Job/get_jobs_all`).
      then((res) => res.json()).
      then(setJobList).
      catch(console.error);
      },[]);


 
  const hasInitializedPage = useRef(false); 
  // Fetch work hours when staff or selectedDate changes
  useEffect(() => {
    console.log('staff or date changed');
     if (!selectedStaffId) {
      if (hasInitializedPage.current) {
        alert('No selected staff');
      }
      return;
    }
    refreshWorkHours();
  }, [selectedStaffId, selectedDate]); //when either changed, trigger refresh

  useEffect(() => {
    hasInitializedPage.current = true; // set to true after first mount
  }, []);


return (
  <div className="workhour-layout">
    {/* LEFT PANEL */}
    <div className="left-panel">
      <div className="form-section">
        <label>Date:</label>
        <input
          type="date"
          value={selectedDate.toISOString().split("T")[0]}
          onChange={(e) => setSelectedDate(new Date(e.target.value))}
        />
      </div>

      <div className="form-section">
        <label>Staff:</label>
        <select
          value={selectedStaffId ?? ""}
          onChange={(e) => {
            setSelectedStaffId(Number(e.target.value));
            
          }}
        >
          <option value="" disabled>Choose staff</option>
          {staffList.map((staff) => (
            <option key={staff.staffId} value={staff.staffId}>{staff.name}</option>
          ))}
        </select>
      </div>

      <div className="form-section">
        <label>Job:</label>
        <select value={selectedJobId ?? ""}onChange={(e) => setSelectedJobId(Number(e.target.value))}>
          <option value="" disabled>Choose Job</option>
          {
            JobList.map((job)=>
            <option key={job.jobId} value={job.jobId}>{job.name}</option>
            )

          }

        </select>
      </div>
<div className="form-section">
  <label>Task Description (max {maxChar_des} chars):</label>
  <input
    type="text"
    value={taskDescription}
    maxLength={maxChar_des} // UI enforcement
    onChange={(e) => setTaskDescription(e.target.value)}
    placeholder="Describe the task"
  />
</div>

<div className="form-section">
  <label>Start Date:</label>
  <input
    type="date"
    value={startDate}
    onChange={(e) => setStartDate(e.target.value)}
  />
</div>

<div className="form-section">
  <label>End Date:</label>
  <input
    type="date"
    value={endDate}
    onChange={(e) => setEndDate(e.target.value)}
  />
</div>

<div className="form-section">
  <label>Start Time:</label>
  <input
    type="time"
    value={startTime}
    onChange={(e) => setStartTime(e.target.value)}
  />
</div>

<div className="form-section">
  <label>End Time:</label>
  <input
    type="time"
    value={endTime}
    onChange={(e) => setEndTime(e.target.value)}
  />
</div>



      <div className="form-section button-group">
        <button onClick={handleSetWorkTime}>Set Work Time</button>
        <button onClick={handleCancelWorkTime}>Cancel Work Time</button>
        <button onClick={handleQueryRemainingMinutes}>Query Remaining Time</button>
        <button onClick={handleQueryOvertime}>Over Work Days Check</button>
      </div>
    </div>

    {/* RIGHT PANEL */}
    <div className="calendar-view">
  <div className="calendar-header-empty"></div>
  {weekDates.map((date, i) => (
    <div key={i} className="calendar-header">
      {date.toLocaleDateString("en-GB", { weekday: "short", day: "numeric" })}
    </div>
  ))}

  <div className="calendar-body">
    <div className="time-column">
      {Array.from({ length: 24 }).map((_, hour) => (
        <div key={hour} className="hour-label">{`${hour}:00`}</div>
      ))}
    </div>

    {weekDates.map((date, col) => {
      const dateStr = date.toISOString().split("T")[0];
      const blocks = workHours.filter(wh => wh.date === dateStr);

      return (
        <div key={col} className="day-column">
          {blocks.map((block, i) => {
            const startMin = parseTime(block.startTime);
            const endMin = parseTime(block.endTime);
            const top = (startMin / 60) * 40;       // 40px per hour
            const height = ((endMin - startMin) / 60) * 40;

            return (
              <div
                key={i}
                className="hour-block"
                style={{ top: `${top}px`, height: `${height}px` }}
              >
                {block.taskDescription || "Task"}
              </div>
            );
          })}
        </div>
      );
    })}
  </div>
    </div>

  </div>
);

}

// Parse "HH:mm" string to minutes since midnight
function parseTime(timeStr) {
  const [h, m] = timeStr.split(":").map(Number);
  return h * 60 + m;
}


export default WorkHourViewer;
