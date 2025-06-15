import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import WorkHourViewer from './StaffSelect.jsx'
createRoot(document.getElementById('root')).render(

    <WorkHourViewer />
    // <StrictMode>
    //   <WorkHourViewer />
    // </StrictMode>

)
