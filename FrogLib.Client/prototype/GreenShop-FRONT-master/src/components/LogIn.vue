<script setup>
    import { ref } from 'vue';

    const isLogin = ref(true);
    const loginData = { email: "", password: "" };
    const registerData = { username: "", email: "", password: "", confirmPassword: "" };

    const API_URL = "https://localhost:7233/API";

    // Функция для входа
    const handleLogin = async () => {
    try {
        const response = await fetch(`https://localhost:7233/API/User/LogIn?login=${loginData.email}&password=${loginData.password}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const data = await response.json();
            console.log("Login success:", data);
        } else {
            console.error("Login failed:", response.statusText);
        }
    } catch (error) {
        console.error("Login error:", error);
    }
};


    // Функция для регистрации
    const handleRegister = async () => {
        try {
            const response = await fetch(`${API_URL}/User/SignUp`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    NameUser: registerData.username,
                    LoginUser: registerData.email,
                    PasswordUser: registerData.password,
                }),
            });

            if (response.ok) {
                const data = await response.json();
                console.log("Registration success:", data);
            } else {
                console.error("Registration failed:", response.statusText);
            }
        } catch (error) {
            console.error("Registration error:", error);
        }
    };
</script>


<template>
    <div class="modal fade" id="authModal" tabindex="-1" aria-labelledby="authModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content custom-modal">
          <div class="modal-header border-0">
            <h5 class="modal-title text-center" id="authModalLabel">
                <span :class="{ active: isLogin }" @click="isLogin = true">Login</span> |
                <span :class="{ active: !isLogin }" @click="isLogin = false">Register</span>
            </h5>

            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div v-if="isLogin">
              <p>Enter your username and password to login.</p>
              <form @submit.prevent="handleLogin">
                <div class="form-group mb-3">
                  <input type="email" class="form-control" placeholder="Enter your email address" v-model="loginData.email" />
                </div>
                <div class="form-group mb-3 position-relative">
                  <input type="password" class="form-control" placeholder="Password" v-model="loginData.password"/>
                </div>
                <div class="text-end">
                  <a href="#" class="forgot-password">Forgot Password?</a>
                </div>
                <button type="submit" class="btn btn-primary w-100">Login</button>
              </form>
            </div>
            <div v-else>
              <p>Enter your email and password to register.</p>
              <form @submit.prevent="handleRegister">
                <div class="form-group mb-3">
                  <input type="text" class="form-control" placeholder="Username" v-model="registerData.username" />
                </div>
                <div class="form-group mb-3">
                  <input type="email" class="form-control" placeholder="Enter your email address" v-model="registerData.email"/>
                </div>
                <div class="form-group mb-3 position-relative">
                  <input type="password" class="form-control" placeholder="Password" v-model="registerData.password"/>
                </div>
                <div class="form-group mb-3">
                  <input type="password" class="form-control" placeholder="Confirm Password" v-model="registerData.confirmPassword"/>
                </div>
                <button type="submit" class="btn btn-success w-100">Register</button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
</template>
  

<style scoped>

    @import url('https://fonts.googleapis.com/css?family=Kanit');

    * {
        font-family: 'Kanit', sans-serif;
    }

    .custom-button {
        background-color: #46A358;
        border-color: #46A358;
        color: white;
        padding: 10px 20px;
        border-radius: 5px;
        font-size: 16px;
    }

    .custom-button:hover {
        background-color: #46A358;
        border-color: #46A358;
    }
    .custom-modal {
        border-radius: 12px;
        padding: 20px;
    }

    .modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .modal-title {
        text-align: center; 
        font-weight: 100;   
        font-size: 20px;    
        /* margin: 0 auto;      */
        width: 90%;
    }

    .modal-title span {
        padding: 0 8px;     
        cursor: pointer;    
    }

    .modal-title span.active {
        color: #46A358;
        font-weight: 400;
    }

    .form-control {
        border: 1px solid #D9D9D9;
        border-radius: 8px;
        padding: 12px 16px;
        font-weight: 100px;
    }

    .form-control:focus {
        outline: none;
        border-color: #46A358;
        box-shadow: 0 0 0 2px rgba(70, 163, 88, 0.25);
        font-weight: 100px;
    }

    .forgot-password {
        font-size: 14px;
        font-weight: 50px;
        color: #46A358;
        text-decoration: none;
    }

    .forgot-password:hover {
        text-decoration: underline;
    }

    .toggle-password {
        position: absolute;
        right: 10px;
        top: 50%;
        transform: translateY(-50%);
        cursor: pointer;
    }

    .btn-primary {
        background-color: #46A358;
        border: none;
        border-radius: 8px;
        font-size: 16px;
        font-weight: 600;
    }

    .btn-primary:hover {
        background-color: #3e924e;
    }

    .btn-success {
        background-color: #46A358;
        border: none;
        border-radius: 8px;
        font-size: 16px;
        font-weight: 600;
    }

    .btn-success:hover {
        background-color: #46A358;
    }

</style>