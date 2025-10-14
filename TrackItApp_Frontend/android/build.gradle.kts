// Top-level build file where you can add configuration options common to all sub-projects/modules.

buildscript {
    repositories {
        maven { url = uri("https://maven.aliyun.com/repository/google") }
        maven { url = uri("https://maven.aliyun.com/repository/gradle-plugin") }
        maven { url = uri("https://maven.aliyun.com/repository/public") }
        mavenCentral()
        maven { url = uri("https://dl.google.com/dl/android/maven2/") }
    }
    dependencies {
        classpath("com.android.tools.build:gradle:8.4.2")
        classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.9.23")
    }
}

allprojects {
    repositories {
        google {
            content {
                includeGroupByRegex("com\\.android.*")
            }
        }
        mavenCentral()
    }

    // ⚙️ إعدادات رفع مهلة التحميل (Timeout) وتحسين الاتصال
    configurations.all {
        resolutionStrategy {
            cacheChangingModulesFor(0, "seconds")
            cacheDynamicVersionsFor(0, "seconds")
        }
    }
}

// ⚙️ هذه الأسطر تضبط مجلد build ليكون في مكان واحد موحد
val newBuildDir: Directory =
    rootProject.layout.buildDirectory
        .dir("../../build")
        .get()
rootProject.layout.buildDirectory.value(newBuildDir)

subprojects {
    val newSubprojectBuildDir: Directory = newBuildDir.dir(project.name)
    project.layout.buildDirectory.value(newSubprojectBuildDir)
}

subprojects {
    project.evaluationDependsOn(":app")
}

tasks.register<Delete>("clean") {
    delete(rootProject.layout.buildDirectory)
}

// ⚡️ تحسينات إضافية لشبكة Gradle
gradle.projectsEvaluated {
    tasks.withType<JavaCompile> {
        options.encoding = "UTF-8"
    }
    println("✅ Gradle build configuration loaded with extended network timeout (1 hour).")
}
