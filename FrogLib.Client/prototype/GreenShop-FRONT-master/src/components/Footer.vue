<template>
    <footer class="footer">
        <div class="footer-content">
            <div class="footer-section about">
                <h4>Garden Care</h4>
                    <p>We are an online plant shop offering a wide range of cheap and trendy plants.</p>
                </div>
                <div class="footer-section about">
                    <h4>Plant Renovation</h4>
                    <p>We are an online plant shop offering a wide range of cheap and trendy plants.</p>
                </div>
                <div class="footer-section about">
                    <h4>Watering Garden</h4>
                    <p>We are an online plant shop offering a wide range of cheap and trendy plants.</p>
                </div>
                <div class="footer-section newsletter">
                    <h4>Would you like to join newsletters?</h4>
                <div class="div-email">
                    <input type="email" style="height: 20px;" placeholder="Enter your email address...">
                    <button type="submit" class="green-button">Join</button>
                </div>
                <p>We usually post offers and challenges in our newsletter. We offer a wide range of houseplants and accessories shipped directly from our (green)house to yours!</p>
            </div>
        </div>

        <div class="footer-bottom">
            <div class="footer-brand">
                <img src="@/assets/Logo.png" alt="GreenShop Logo">
                <p>70 West Buckingham Ave., Farmingdale, NY 11735</p>
                <p>contact@greenshop.com</p>
                <p>+88 01911 717 490</p>
            </div>

            <div class="footer-links">
                <div class="footer-column">
                    <h4>My Account</h4>
                    <ul>
                        <li><a>My Account</a></li>
                        <li><a>Our stores</a></li>
                        <li><a>Contact us</a></li>
                        <li><a>Career</a></li>
                        <li><a>Specials</a></li>
                    </ul>
                </div>
                <div class="footer-column">
                    <h4>Help & Guide</h4>
                    <ul>
                        <li><a>Help Center</a></li>
                        <li><a>How to Buy</a></li>
                        <li><a>Shipping & Delivery</a></li>
                        <li><a>Product Policy</a></li>
                        <li><a>How to Return</a></li>
                    </ul>
                </div>
                <div class="footer-column">
                    <h4>Categories</h4>
                    <ul>
                        <li><a>House Plants</a></li>
                        <li><a>Potter Plants</a></li>
                        <li><a>Seeds</a></li>
                        <li><a>Small Plants</a></li>
                        <li><a>Accessories</a></li>
                    </ul>
                </div>
                <div class="footer-column social-media">
                    <h4>Social Media</h4>
                    <div class="social-icons">
                        <a><img src="@/assets/icons/Facebook.png" alt="Facebook"></a>
                        <a><img src="@/assets/icons/Instagram.png" alt="Instagram"></a>
                        <a><img src="@/assets/icons/Twitter.png" alt="Twitter"></a>
                        <a><img src="@/assets/icons/Linkedin.png" alt="LinkedIn"></a>
                        <a><img src="@/assets/icons/Union.png" alt="Union"></a>
                    </div>
                    <div class="payment-icons">
                        <img src="@/assets/payment.png" alt="Payment">
                    </div>
                </div>
            </div>
        </div>

        <div class="footer-copy">
            <p>2021 GreenShop. All Rights Reserved.</p>
        </div>
    </footer>
</template>

<style>
    @import url('https://fonts.googleapis.com/css?family=Kanit');

    * {
        font-family: 'Kanit';
    }

    .footer {
        background-color: #f5f5f5;
        padding: 40px;
    }

    .footer-content {
        display: flex;
        justify-content: space-between;
        margin-bottom: 40px;
    }

    .footer-section {
        width: 22%;
    }

    .footer-section h4 {
        color: #46A358;
        font-size: 20px;
        margin-bottom: 10px;
    }

    .footer-section p {
        font-size: 14px;
        color: #555;
    }

    /* .newsletter form {
        display: flex;
        gap: 10px;
    } */

    .footer-bottom {
        display: flex;
        justify-content: space-between;
        margin-bottom: 20px;
    }

    .footer-brand {
        width: 30%;
        font-size: 14px;
    }

    .footer-links {
        display: flex;
        width: 70%;
        justify-content: space-between;
    }

    .footer-column ul {
        list-style: none;
        padding: 0;
    }

    .footer-column ul li {
        margin: 5px 0;
    }

    .footer-column ul li a {
        text-decoration: none;
        color: #555;
        font-size: 14px;
    }

    .footer-column .social-media {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
    }

    .social-icons {
        display: flex;
        gap: 10px;
        margin-bottom: 10px;
        width: 30px;
    }

    .payment-icons img {
        margin-right: 10px;
        width: 224px;
    }

    .footer-copy {
        text-align: center;
        color: #999;
        font-size: 14px;
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

    .div-email {
        display: flex;
        height: 40px;
    }

    .div-email input {
        width: 270px;
        border: none;
    }

    .div-email button {
        width: 85px;
        background-color: #46A358;
        font-weight: bold;
        color: white;
        border-radius: 0 10px 10px 0;
        border: none;
        cursor: pointer;
    }
</style>