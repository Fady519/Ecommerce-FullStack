import { lazy, Suspense } from "react";
import { Toaster } from "react-hot-toast";
import BtmHeader from "./components/Header/BtmHeader";
import TopHeader from "./components/Header/TopHeader";
import { Routes, Route, useLocation } from "react-router-dom";
import ScrollToTop from "./components/ScrollToTop";
import { AnimatePresence } from "framer-motion";
import ProtectedRoute from "./page/ProtectedRoute";
import AdminRoute from "./page/Admin/AdminRoute";
import PageTitleManager from "./components/PageTitleManager";
import RouteLoading from "./components/RouteLoading";

// Eager load Home for instant First Contentful Paint
import Home from "./page/home/Home";

// Lazy-loaded routes for code-splitting and Lighthouse optimization
const Cart = lazy(() => import("./page/Cart/Cart"));
const ProductDeatails = lazy(() => import("./page/home/ProductDetails/ProductDeatails"));
const CategoryPage = lazy(() => import("./page/CategoryPage/CategoryPage"));
const SearchResults = lazy(() => import("./page/SearchResults"));
const Favorites = lazy(() => import("./page/Favorites/Favorites"));
const About = lazy(() => import("./page/About/About"));
const Contact = lazy(() => import("./page/Contact/Contact"));
const BlogList = lazy(() => import("./page/Blog/BlogList"));
const BlogDetail = lazy(() => import("./page/Blog/BlogDetails"));
const Accessories = lazy(() => import("./page/Accessories/Accessories"));
const Login = lazy(() => import("./components/Login/Login"));
const Register = lazy(() => import("./components/Register/Register"));
const Checkout = lazy(() => import("./page/Checkout/Checkout"));
const MyOrders = lazy(() => import("./page/Orders/MyOrders"));
const AdminDashboard = lazy(() => import("./page/Admin/AdminDashboard"));
const NotFound = lazy(() => import("./page/NotFound/NotFound"));

function App() {
  const location = useLocation();

  return (
    <>
      <PageTitleManager />
      <header>
        <TopHeader />
        <BtmHeader />
      </header>

      <Toaster position="bottom-right" toastOptions={{
        style: {
          background: '#333',
          color: '#fff',
          borderRadius: '8px',
          padding: '14px'
        }
      }} />

      <ScrollToTop />

      <Suspense fallback={<RouteLoading />}>
        <AnimatePresence mode="wait">
          <Routes location={location} key={location.pathname}>
            
            {/* Public browsing routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/search" element={<SearchResults />} />
            <Route path="/products/:id" element={<ProductDeatails />} />
            <Route path="/category" element={<CategoryPage />} />
            <Route path="/category/:category" element={<CategoryPage />} />
            <Route path="/about" element={<About />} />
            <Route path="/contact" element={<Contact />} />
            <Route path="/blog" element={<BlogList />} />
            <Route path="/blog/:id" element={<BlogDetail />} />
            <Route path="/accessories" element={<Accessories />} />

            {/* Authenticated member routes */}
            <Route path="/cart" element={<ProtectedRoute><Cart /></ProtectedRoute>} />
            <Route path="/checkout" element={<ProtectedRoute><Checkout /></ProtectedRoute>} />
            <Route path="/orders" element={<ProtectedRoute><MyOrders /></ProtectedRoute>} />
            <Route path="/favorites" element={<ProtectedRoute><Favorites /></ProtectedRoute>} />

            {/* Admin portal */}
            <Route path="/admin" element={<AdminRoute><AdminDashboard /></AdminRoute>} />

            {/* 404 Not Found Catch-All */}
            <Route path="*" element={<NotFound />} />

          </Routes>
        </AnimatePresence>
      </Suspense>
    </>
  );
}

export default App;