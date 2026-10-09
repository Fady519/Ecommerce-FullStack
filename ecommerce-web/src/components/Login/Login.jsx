import React from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast'; // استيراد التوستر مهم جداً هنا
import './Login.css';

export default function Login() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors } } = useForm();

  const onSubmit = (data) => {
    // 🔵 بنجيب البيانات اللي اتخزنت وقت الريجستر
    const savedUser = JSON.parse(localStorage.getItem("registeredUser"));

    // 🔵 بنشيك: هل الايميل والباسورد مطابقين للي اتخزن؟
    if (savedUser && savedUser.email === data.email && savedUser.password === data.password) {
      localStorage.setItem("user", "true"); // دي اللي بتفتح الـ ProtectedRoute
      toast.success(`Welcome back, ${savedUser.fullName}!`);
      
      // نقله للهوم وعمل ريفرش لتحديث حالة الهيدر
      setTimeout(() => {
        navigate("/");
        window.location.reload();
      }, 1000);
    } else {
      // 🔴 لو البيانات غلط أو مش موجودة أصلاً
      toast.error("Invalid email or password. Please try again!");
    }
  };

  return (
    <div className="auth_container">
      <div className="auth_card">
        <h2>Welcome Back!</h2>
        <p>Please enter your details to login.</p>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="input_group">
            <label>Email Address</label>
            <input 
              type="email" 
              placeholder="example@mail.com"
              {...register("email", { 
                required: "Email is required", 
                pattern: { value: /^\S+@\S+$/i, message: "Invalid email address" } 
              })} 
            />
            {errors.email && <span className="error_msg">{errors.email.message}</span>}
          </div>

          <div className="input_group">
            <label>Password</label>
            <input 
              type="password" 
              placeholder="••••••••"
              {...register("password", { 
                required: "Password is required", 
                minLength: { value: 6, message: "Min length is 6 characters" } 
              })} 
            />
            {errors.password && <span className="error_msg">{errors.password.message}</span>}
          </div>

          <button type="submit" className="main_btn">Login</button>
        </form>

        <div className="auth_footer">
          <p>Don't have an account? <Link to="/register">Create Account</Link></p>
        </div>
      </div>
    </div>
  );
}