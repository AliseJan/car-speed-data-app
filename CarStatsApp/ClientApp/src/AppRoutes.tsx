import CarStatsPage from "./components/CarStatsPage/CarStatsPage";
import AverageSpeedPage from "./components/AverageSpeedPage/AverageSpeedPage";
import Home from "./components/HomePage/Home";
import 'react-toastify/dist/ReactToastify.css';

const AppRoutes = [
  {
    path: '/',
    element: <Home />
  },
  {
    path: '/car-stats',
    element: <CarStatsPage />
  },
  {
    path: '/average-speed',
    element: <AverageSpeedPage />
  },
];

export default AppRoutes;