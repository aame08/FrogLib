<template>
    <div class="div-main">
      <p class="text-welcome">Welcome to GreenShop</p>
      <p class="text-bold">
        Let’s Make <br>
        a Better <span style="color: #46A358;">Planet</span>
      </p>
      <p class="text-desc">
        We are an online plant shop offering a wide range of cheap and trendy plants.<br>
        Use our plants to create a unique Urban Jungle. Order your favorite plants!
      </p>
  
      <button class="green-button">
        <span>Shop now</span>
      </button>
  
      <div class="div-hero">
        <div class="button-container right">
          <img src="@/assets/Green plant.png" alt="Right Plant">
        </div>
        <div class="button-container bottom-left">
          <img src="@/assets/Green plant.png" alt="Bottom Left Plant">
        </div>
      </div>
    </div>
  </template>
  
  <style>
  @import url('https://fonts.googleapis.com/css?family=Kanit');

    * {
        font-family: 'Kanit', sans-serif;
    }

  .div-main {
        background-color: #F5F5F580;
        padding: 10px;
        margin-left: 20px;
        margin-right: 20px;
    }

    .text-welcome {
        text-transform: uppercase; 
        font-size: 18px;
        margin-left: 50px;
        margin-top: 50px;
        margin-bottom: 0%;
    }

    .text-bold {
        text-transform: uppercase;
        font-size: 70px;
        font-weight: bold;
        margin-bottom: 5px;
        margin-top: 0%;
        margin-left: 50px;
    }

    .text-desc {
        font-size: 18px;
        margin-left: 50px;
        margin-top: 0%;
        margin-bottom: 0%;
    }

    .green-button {
      background-color: #46A358;
      font-size: 18px;
      color: white;
      padding: 10px 20px;
      border-radius: 5px;
      border: none;
      cursor: pointer;
      text-transform: uppercase;
      margin-top: 45px;
      margin-left: 50px;
    }
  
    .div-hero {
      display: flex;
      position: relative;
    }
  
    .button-container {
      position: absolute;
    }
    
    .button-container.right {
      top: 50%;
      right: 15%;
      transform: translateY(-94%);
      width: 518px; 
      height: 518px;
    }
    
    .button-container.bottom-left {
      transform: translateY(-105%);
      left: 52%; 
      width: 135px; 
      height: 135px; 
    }
    
    .button-container img {
      width: 100%; 
      height: auto;
    }
  </style>
  