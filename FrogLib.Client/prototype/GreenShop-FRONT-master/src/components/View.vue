<template>
    <div class="container">
       <div class="sidebar">
         <h2>Categories</h2>
         <ul class="categories-list">
            <li class="active">House Plants <span class="category-count">(33)</span></li>
            <li>Potter Plants <span class="category-count">(12)</span></li>
            <li>Seeds <span class="category-count">(65)</span></li>
            <li>Small Plants <span class="category-count">(39)</span></li>
            <li>Big Plants <span class="category-count">(23)</span></li>
            <li>Succulents <span class="category-count">(17)</span></li>
            <li>Terrariums <span class="category-count">(19)</span></li>
            <li>Gardening <span class="category-count">(13)</span></li>
            <li>Accessories <span class="category-count">(18)</span></li>
         </ul>
    
         <div class="filter-section">
           <h3>Price Range</h3>
           <div class="price-filter">
             <input type="range" min="39" max="1230" value="500" style="width: 200px;">
           </div>
           <p class="price-range">Price: $39 - $1230</p>
           <button class="green-button">Filter</button>
         </div>
    
         <div class="filter-section">
           <h3>Size</h3>
           <ul class="filter-size">
             <li>Small <span class="category-count">(119)</span></li>
             <li>Medium <span class="category-count">(186)</span></li>
             <li>Large <span class="category-count">(78)</span></li>
           </ul>
         </div>
         <div class="sale-banner">
            <button class="sale-button">
                <img src="@/assets/Super Sale Banner.png" alt="Super Sale">
            </button>
         </div>
       </div>
    

       <div class="main-content">
        <div class="products-nav">
            <ul class="nav-links">
                <li class="active-link">All Plants</li>
                <li>New Arrivals</li>
                <li>Sale</li>
            </ul>
            <div class="sorting">
            <label for="sorting">Sort by:</label>
            <select id="sorting">
                <option value="default">Default sorting</option>
                <option value="price-asc">Price: Low to High</option>
                <option value="price-desc">Price: High to Low</option>
                <option value="newest">Newest Arrivals</option>
            </select>
            </div>
        </div>
         <h3>All Plants</h3>
         <div class="products-grid">
           <div class="product-card">
             <img src="@/assets/Plants/plant 1.png" alt="Barberton Daisy">
             <h4>Barberton Daisy</h4>
             <p class="price">$119.00</p>
           </div>
    
           <div class="product-card">
             <img src="@/assets/Plants/plant 2.png" alt="Angel Wing Begonia">
             <h4>Angel Wing Begonia</h4>
             <p class="price">$169.00</p>
           </div>
    
           <div class="product-card">
             <img src="@/assets/Plants/plant 3.png" alt="African Violet">
             <h4>African Violet</h4>
             <p class="price">$199.00 <span class="old-price">$229.00</span></p>
           </div>
    
           <div class="product-card">
             <img src="@/assets/Plants/plant 4.png" alt="Beach Spider Lily">
             <h4>Beach Spider Lily</h4>
             <p class="price">$129.00</p>
           </div>
    
           <div class="product-card">
             <img src="@/assets/Plants/plant 5.png" alt="Brushing Bromeliad">
             <h4>Brushing Bromeliad</h4>
             <p class="price">$139.00</p>
           </div>
    
           <div class="product-card">
             <img src="@/assets/Plants/plant 6.png" alt="Aluminium Plant">
             <h4>Aluminium Plant</h4>
             <p class="price">$179.00</p>
           </div>
         </div>
       </div>
     </div>
    </template>
    
    <style>
      @import url('https://fonts.googleapis.com/css?family=Kanit');

       * {
         font-family: 'Kanit';
       }
    
       body {
         background-color: #F5F5F580;
         padding: 20px;
       }
    
       .container {
         display: flex;
       }
    
       .sidebar {
         width: 25%;
         padding: 20px;
       }
    
       .sidebar h3 {
         font-size: 24px;
         font-weight: bold;
       }
    
       .categories-list,
       .filter-size {
         list-style: none;
         padding: 0;
       }
    
       .categories-list li,
       .filter-size li {
         margin: 10px 0;
         font-size: 15px;
       }
    
       .categories-list li.active {
         color: #46A358;
         font-weight: bold;
       }
    
       .category-count {
         color: gray;
       }
    
       .filter-section {
         margin: 30px 0;
       }
    
       .price-range {
         font-size: 15px;
         margin-bottom: 10px;
       }
    
       .price-filter input {
         width: 100%;
         margin: 10px 0;
       }
    
       .green-button {
         background-color: #46A358;
         font-size: 18px;
         color: white;
         border-radius: 5px;
         border: none;
         cursor: pointer;
         text-transform: uppercase;
       }
    
       .main-content {
         width: 75%;
         padding: 20px;
       }
    
       .products-grid {
         display: grid;
         grid-template-columns: repeat(3, 1fr);
         gap: 20px;
       }
    
       .product-card {
         background-color: white;
         padding: 20px;
         border-radius: 8px;
         text-align: left;
       }
    
       .product-card img {
         width: 250px; 
         height: auto;
       }
    
       .product-card h4 {
         margin-top: 10px;
         font-size: 16px;
       }
    
       .product-card .price {
         font-size: 16px;
         margin-top: 5px;
         color: #46A358;
       }
    
       .product-card .old-price {
         text-decoration: line-through;
         color: gray;
         font-size: 14px;
       }

       .sale-banner {
            margin: 30px 0;
            text-align: center;
        }

        .sale-button {
            background: none;
            border: none;
            cursor: pointer;
            position: relative;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .sale-button img {
            width: 100%;
            max-width: 370px; 
            border-radius: 10px;
        }

        .sale-button:hover {
            transform: scale(1.05); 
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2); 
        }

        .products-nav {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .nav-links {
            display: flex;
            list-style: none;
            gap: 20px;
        }

        .nav-links li {
            cursor: pointer;
            font-size: 16px;
            color: black;
            font-weight: normal;
            transition: color 0.3s ease;
        }

        .nav-links li.active-link {
            color: #46A358;
            border-bottom: 2px solid #46A358;
            padding-bottom: 2px;
        }

        .nav-links li:hover {
            color: #46A358;
        }

        .sorting {
            display: flex;
            align-items: center;
        }

        .sorting label {
            margin-right: 10px;
            font-size: 16px;
        }

        .sorting select {
            padding: 5px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

    </style>
    