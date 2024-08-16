plugins {
    id("com.android.application")
}

android {
    namespace = "com.example.greengrowtechapp"
    compileSdk = 33

    defaultConfig {
        applicationId = "com.example.greengrowtechapp"
        minSdk = 31
        targetSdk = 33
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(getDefaultProguardFile("proguard-android-optimize.txt"), "proguard-rules.pro")
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_1_8
        targetCompatibility = JavaVersion.VERSION_1_8
    }
    buildFeatures {
        viewBinding = true
    }
}

dependencies {
    implementation("org.jetbrains.kotlin:kotlin-stdlib:1.8.10") // Ensure all Kotlin stdlib dependencies are the same version
    implementation("androidx.appcompat:appcompat:1.6.1")
    implementation("androidx.activity:activity:1.7.2")
    implementation("com.google.android.material:material:1.9.0") // Ensure this version supports SDK 33
    implementation("androidx.constraintlayout:constraintlayout:2.1.4")
    implementation("androidx.lifecycle:lifecycle-livedata-ktx:2.6.1")
    implementation("androidx.lifecycle:lifecycle-viewmodel-ktx:2.6.1")
    implementation("androidx.navigation:navigation-fragment:2.5.3")
    implementation("androidx.navigation:navigation-ui:2.5.3")
    implementation("com.android.volley:volley:1.2.1")
    testImplementation("junit:junit:4.13.2")
    androidTestImplementation("androidx.test.ext:junit:1.1.5")
    androidTestImplementation("androidx.test.espresso:espresso-core:3.5.1")
}

configurations.all {
    resolutionStrategy {
        force ("org.jetbrains.kotlin:kotlin-stdlib:1.8.10")
        force ("org.jetbrains.kotlin:kotlin-stdlib-jdk7:1.8.10")
        force ("org.jetbrains.kotlin:kotlin-stdlib-jdk8:1.8.10")
    }
}
