import React, { useContext, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import toast from 'react-hot-toast';
import { AuthContext } from '../Context/AuthContext';
import { ApiError } from '../../services/api';
import './Login.css';

export default function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useContext(AuthContext);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { register, handleSubmit, setError, formState: { errors } } = useForm();

  const from = location.state?.from?.pathname || "/";

  const onSubmit = async (data) => {
    setIsSubmitting(true);
    try {
      const user = await login(data.email, data.password);
      toast.success(`Welcome back, ${user.displayName}!`);
      navigate(from, { replace: true });
    } catch (err) {
      if (err instanceof ApiError && err.errors) {
        Object.keys(err.errors).forEach((key) => {
          const lowerKey = key.toLowerCase();
          if (lowerKey.includes('email')) {
            setError('email', { type: 'server', message: err.errors[key][0] });
          } else if (lowerKey.includes('password')) {
            setError('password', { type: 'server', message: err.errors[key][0] });
          }
        });
      } else {
        toast.error(err.message || "Invalid email or password. Please try again!");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="auth_container">
      <div className="auth_card">
        <h2>Welcome Back!</h2>
        <p>Please enter your credentials to login to your store account.</p>

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="input_group">
            <label>Email Address</label>
            <input 
              type="email" 
              placeholder="example@mail.com"
              autoComplete="email"
              className={errors.email ? "input_error" : ""}
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
              autoComplete="current-password"
              className={errors.password ? "input_error" : ""}
              {...register("password", { 
                required: "Password is required", 
                minLength: { value: 6, message: "Min length is 6 characters" } 
              })} 
            />
            {errors.password && <span className="error_msg">{errors.password.message}</span>}
          </div>

          <button type="submit" className="main_btn" disabled={isSubmitting}>
            {isSubmitting ? "Signing in..." : "Login"}
          </button>
        </form>

        <div className="auth_footer">
          <p>Don't have an account? <Link to="/register">Create Account</Link></p>
        </div>
      </div>
    </div>
  );
}