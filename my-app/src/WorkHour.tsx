import { useEffect } from "react";
import { useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;
// WorkHour type
type WorkHour = {
  staffId: number;
  date: string; // "2025-06-17"
  startTime: string; // "08:30"
  endTime: string; // "10:00"
  taskDescription?: string;
  jobId?: number;
  onWork: boolean;
};

const [workHours, setWorkHours] = useState<WorkHour[]>([]);


