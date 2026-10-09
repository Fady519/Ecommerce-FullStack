import { Navigate } from 'react-router-dom';

const ProtectedRoute = ({ children }) => {
    // بنشيك لو اليوزر مسجل دخول (غالباً بنخزن كلمة 'user' أو 'token' في الـ localStorage)
    const isAuthenticated = localStorage.getItem("user"); 

    if (!isAuthenticated) {
        // لو مش مسجل، ابعته لصفحة اللوجن
        return <Navigate to="/login" replace />;
    }

    // لو مسجل، اعرض الصفحة اللي هو عايزها
    return children;
};

export default ProtectedRoute;