<script setup>
  const LogIn_Click = () =>{
    const modal = new bootstrap.Modal(document.getElementById('authModal'));
    modal.show();
  }
</script>

<template>
  <div class="header-container">
    <div class="div-left">
      <img src="@/assets/Logo.png" alt="Logo" class="logo">
    </div>
    <div class="div-center">
        <span>
          <button class="transparent-button">
            <RouterLink to="/">Home</RouterLink>
          </button>
          <button class="transparent-button">
            <RouterLink to="/shop">Shop</RouterLink>
          </button>
          <button class="transparent-button">
            <RouterLink to="/plant_care">Plant care</RouterLink>
          </button>
          <button class="transparent-button">
            <RouterLink to="/blogs">Blogs</RouterLink>
          </button>
        </span>
      </div>
    <div class="div-right">
        <button class="transparent-button">
          <img src="@/assets/Vector.png" alt="Поиск">
        </button>
        <button class="transparent-button">
          <img src="@/assets/Basket.png" alt="Корзина">
        </button>
        <button class="image-text-button" @click="LogIn_Click">
            <!-- <img src="@/assets/Logout.png" alt="Войти"> -->
            <span>Log in</span>
        </button>
    </div>
  </div>
</template>

<style>
    
    @import url('https://fonts.googleapis.com/css?family=Kanit');

    * {
        font-family: 'Kanit', sans-serif;
    }

    a {
        text-decoration: none;
        color: black;
    }
    a:hover {
        color: #46A358;
        text-decoration: underline;
    }
    
    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0 20px;
    }

    .transparent-button {
        background-color: transparent;
        color: #2F2105;
        padding: 10px 20px;
        font-size: 16px;
        border-radius: 5px;
        border: none;
        cursor: pointer;
    }

    .green-button {
        background-color: #46A358;
        color: #46A358;
        padding: 10px 20px;
        border-radius: 5px;
        border: none;
        cursor: pointer;
    }

    .div-center {
        margin: 0 auto;
        width: max-content;
    }

    .div-left {
        display: flex;
        align-items: center;
    }

    .div-right {
        display: flex;
        align-items: center;
    }

    .logo {
        width: 100px;
        height: auto;
    }

    .image-text-button {
        display: flex;
        align-items: center;
        background-color: #46A358;
        color: #FFFF;
        padding: 10px 20px;
        border-radius: 5px;
        border: none;
        cursor: pointer;
    }
</style>
