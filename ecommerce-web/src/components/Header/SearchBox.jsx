import React, { useEffect, useState, useRef } from 'react';
import { FaSearch } from "react-icons/fa";
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { api } from '../../services/api';

function SearchBox() {
    const navigate = useNavigate();
    const location = useLocation();

    const [searchTerm, setSearchTerm] = useState("");
    const [suggestions, setSuggestions] = useState([]);
    const [showSuggestions, setShowSuggestions] = useState(false);
    const boxRef = useRef(null);

    useEffect(() => {
        const queryParam = new URLSearchParams(location.search).get("query") || "";
        if (location.pathname.startsWith("/search")) {
            setSearchTerm(queryParam);
        } else {
            setSearchTerm("");
        }
    }, [location]);

    // Close on click outside
    useEffect(() => {
        const handleClickOutside = (e) => {
            if (boxRef.current && !boxRef.current.contains(e.target)) {
                setShowSuggestions(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (searchTerm.trim()) {
            navigate(`/search?query=${encodeURIComponent(searchTerm.trim())}`);
        }
        setShowSuggestions(false);
    };

    // Fetch live suggestions from real backend API
    useEffect(() => {
        if (!searchTerm.trim()) {
            setSuggestions([]);
            return;
        }

        const debounce = setTimeout(async () => {
            try {
                const res = await api.products.getAll({ search: searchTerm.trim(), pageSize: 6 });
                setSuggestions(res.data || []);
                setShowSuggestions(true);
            } catch {
                setSuggestions([]);
            }
        }, 250);

        return () => clearTimeout(debounce);
    }, [searchTerm]);

    return (
        <div className='searchBox_container' ref={boxRef}>
            <form onSubmit={handleSubmit} className='search_box'>
                <input
                    type="text"
                    name='search'
                    id='search'
                    placeholder='Search for products, brands, or styles...'
                    value={searchTerm}  
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onFocus={() => { if (suggestions.length > 0) setShowSuggestions(true); }}
                    autoComplete='off'
                />
                <button type='submit' aria-label="Search"> <FaSearch /> </button>
            </form>

            {showSuggestions && suggestions.length > 0 && (
                <ul className='suggestation'>
                    {suggestions.map((item) => (
                        <Link 
                            key={item.id} 
                            to={`/products/${item.id}`} 
                            onClick={() => setShowSuggestions(false)}
                        >
                            <li>
                                <img src={item.pictureUrl} alt={item.name} />
                                <div className="suggestion_info">
                                    <span className="title">{item.name}</span>
                                    <span className="meta">{item.productBrand} • ${item.price}</span>
                                </div>
                            </li>
                        </Link>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default SearchBox;