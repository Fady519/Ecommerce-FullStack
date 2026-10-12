import React, { useEffect, useState } from 'react';
import { useLocation, Link } from 'react-router-dom';
import SlideProductLoading from './../components/slideProducts/SlideProductLoading';
import Product from './../components/slideProducts/Product';
import PageTransition from '../components/PageTransition';
import noResults from "../img/no-results.png";
import { api } from '../services/api';
import './CategoryPage/CategoryPage.css';

function SearchResults() {
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(true);

    const location = useLocation();
    const query = new URLSearchParams(location.search).get("query") || "";

    useEffect(() => {
        const fetchResults = async () => {
            setLoading(true);
            try {
                const res = await api.products.getAll({ search: query, pageSize: 50 });
                setResults(res.data || []);
            } catch (error) {
                console.error("Search Error: ", error);
                setResults([]);
            } finally {
                setLoading(false);
            }
        };

        if (query) {
            fetchResults();
        } else {
            setResults([]);
            setLoading(false);
        }
    }, [query]);

    return (
        <PageTransition key={query}>
            <div className='category_products'>
                <div className='container'>
                    <div className='catalog_header_banner'>
                        <div className="banner_text">
                            <span className="catalog_tag">Search Results</span>
                            <h2>"{query}"</h2>
                            <p>Showing verified store items matching your search query.</p>
                        </div>
                        <div className="catalog_stats">
                            <span className="stats_count">{results.length}</span>
                            <span className="stats_label">Matches Found</span>
                        </div>
                    </div>

                    {loading ? (
                        <div className="catalog_loading_grid">
                            <SlideProductLoading key={query} />
                        </div>
                    ) : results.length > 0 ? (
                        <div className='products_grid'>
                            {results.map((item) => (
                                <Product item={item} key={item.id} />
                            ))}
                        </div>
                    ) : (
                        <div className="container no-results-container">
                            <div className="no-results">
                                <img src={noResults} alt="no results" style={{ maxWidth: '180px', margin: '0 auto 20px' }} />
                                <h2>No Results Found</h2>
                                <p>
                                    Sorry, we couldn't find any products matching <span>"{query}"</span>
                                </p>
                                <Link to="/category/all" className="back-home" style={{ display: 'inline-block', marginTop: '15px' }}>
                                    Browse All Catalog
                                </Link>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </PageTransition>
    );
}

export default SearchResults;
