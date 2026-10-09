import { Toaster } from "react-hot-toast";
import BtmHeader from "./components/Header/BtmHeader";
import TopHeader from "./components/Header/TopHeader";
import Cart from "./page/Cart/Cart";
import Home from "./page/home/Home";
import ProductDeatails from "./page/home/ProductDetails/ProductDeatails";
import { Routes, Route, useLocation } from "react-router-dom";
import ScrollToTop from "./components/ScrollToTop";
import { AnimatePresence } from "framer-motion";
import CategoryPage from "./page/CategoryPage/CategoryPage";
import SearchResults from "./page/SearchResults";
import Favorites from "./page/Favorites/Favorites";
import About from "./page/About/About";
import Contact from "./page/Contact/Contact";
import BlogList from "./page/Blog/BlogList";
import BlogDetail from "./page/Blog/BlogDetails";
import Accessories from "./page/Accessories/Accessories";
import Login from "./components/Login/Login";
import Register from "./components/Register/Register";
import ProtectedRoute from "./page/ProtectedRoute";


function App() {
  const location = useLocation();

  return (
    <>
      <header>
        <TopHeader />
        <BtmHeader />
      </header>

      <Toaster position="bottom-right" toastOptions={{
        style: {
          background: '#e9e9e9',
          borderRadius: '5px',
          padding: '14px'
        }
      }} />

      <ScrollToTop />

      <AnimatePresence mode="wait">
        <Routes location={location} key={location.pathname}>
          
          {/* الصفحات المفتوحة للكل */}
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          {/* الصفحات المحمية - لن تفتح إلا بعد تسجيل الدخول */}
          <Route path="/" element={<ProtectedRoute><Home /></ProtectedRoute>} />
          <Route path="/cart" element={<ProtectedRoute><Cart /></ProtectedRoute>} />
          <Route path="/search" element={<ProtectedRoute><SearchResults /></ProtectedRoute>} />
          <Route path="/favorites" element={<ProtectedRoute><Favorites /></ProtectedRoute>} />
          <Route path="/about" element={<ProtectedRoute><About /></ProtectedRoute>} />
          <Route path="/contact" element={<ProtectedRoute><Contact /></ProtectedRoute>} />
          <Route path="/blog" element={<ProtectedRoute><BlogList /></ProtectedRoute>} />
          <Route path="/blog/:id" element={<ProtectedRoute><BlogDetail /></ProtectedRoute>} />
          <Route path="/accessories" element={<ProtectedRoute><Accessories /></ProtectedRoute>} />
          <Route path="/products/:id" element={<ProtectedRoute><ProductDeatails /></ProtectedRoute>} />
          <Route path="/category/:category" element={<ProtectedRoute><CategoryPage /></ProtectedRoute>} />

        </Routes>
      </AnimatePresence>
    </>
  );
}

export default App;