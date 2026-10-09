import React from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast'; // استيراد التوستر للإشعارات
import '../Login/Login.css';

export default function Register() {
  const { register, handleSubmit, watch, formState: { errors } } = useForm();
  const password = watch("password");
  const navigate = useNavigate();

  const onSubmit = (data) => {
    try {
      // 🟢 بنخزن بيانات اليوزر في الجهاز عشان نقارنها في اللوجن
      localStorage.setItem("registeredUser", JSON.stringify(data));
      
      // إظهار إشعار النجاح
      toast.success("Registration successful! Please login.");
      
      // نقله لصفحة اللوجن بعد ثانية واحدة
      setTimeout(() => {
        navigate("/login");
      }, 1000);
    } catch (error) {
      toast.error("Something went wrong!");
    }
  };

  return (
    <div className="auth_container">
      <div className="auth_card">
        <h2>Create Account</h2>
        <p>Join us today! It only takes a minute.</p>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="input_group">
            <label>Full Name</label>
            <input 
              type="text" 
              placeholder="John Doe"
              {...register("fullName", { required: "Full Name is required" })} 
            />
            {errors.fullName && <span className="error_msg">{errors.fullName.message}</span>}
          </div>

          <div className="input_group">
            <label>Email Address</label>
            <input 
              type="email" 
              placeholder="example@mail.com"
              {...register("email", { 
                required: "Email is required", 
                pattern: { value: /^\S+@\S+$/i, message: "Invalid email" } 
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
                minLength: { value: 6, message: "Min 6 characters" } 
              })} 
            />
            {errors.password && <span className="error_msg">{errors.password.message}</span>}
          </div>

          <div className="input_group">
            <label>Confirm Password</label>
            <input 
              type="password" 
              placeholder="••••••••"
              {...register("confirmPassword", { 
                required: "Please confirm your password",
                validate: value => value === password || "Passwords do not match"
              })} 
            />
            {errors.confirmPassword && <span className="error_msg">{errors.confirmPassword.message}</span>}
          </div>

          <button type="submit" className="main_btn">Create Account</button>
        </form>

        <div className="auth_footer">
          <p>Already have an account? <Link to="/login">Login here</Link></p>
        </div>
      </div>
    </div>
  );
}