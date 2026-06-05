# Railway + Neon Postgres Deployment Guide

## What I've Done

✅ Backend code updated for Railway  
✅ Npgsql (Postgres driver) added  
✅ Dockerfile created  
✅ appsettings.json secured (no secrets)  
✅ Port binding configured  
✅ CORS for frontend URL support

---

## Step 1: Create Neon Database

1. Go to **https://neon.tech**
2. Sign up with GitHub
3. Create a new project
4. A default database and branch will be created
5. Copy the connection string that looks like:
   ```
   postgresql://username:password@ep-xxxxx.us-east-1.neon.tech/dbname?sslmode=require
   ```

   - Click "Connection string" → "Nodejs" or copy the full URL
6. **Keep this safe** — this is your `DATABASE_URL`

---

## Step 2: Create Railway Project

1. Go to **https://railway.app**
2. Click **"Dashboard"** → **"New Project"**
3. Choose **"Deploy from GitHub Repo"**
4. Select your GitHub account and authorize Railway
5. Find and select the `frontier-back` repo
6. Leave the root path blank (Railway will find the Dockerfile)

---

## Step 3: Configure Railway Environment Variables

In Railway dashboard, click on your newly created service and go to **"Variables"**:

Add these variables **exactly as shown**:

| Name                     | Value                                                            |
| ------------------------ | ---------------------------------------------------------------- |
| `DATABASE_URL`           | Your Neon connection string from Step 1                          |
| `FRONTEND_URL`           | `http://localhost:5173` (for now; update after frontend deploys) |
| `Auth:Key`               | A random string (e.g., `super-secret-jwt-key-change-me-in-prod`) |
| `Auth:Issuer`            | `BlogApi`                                                        |
| `Auth:Audience`          | `BlogApiClients`                                                 |
| `ASPNETCORE_ENVIRONMENT` | `Production`                                                     |

### How to add variables:

- Click the "+" button next to "Variables"
- Enter the key and value
- Press Enter or click Save

---

## Step 4: Verify Build and Deploy

1. Railway automatically detects the Dockerfile
2. It builds and deploys
3. Watch the **"Deployments"** tab for status
4. Once **"Running"** appears, your backend is live

### Get your Railway URL:

- Go to the service
- In the top right, click **"Copy Domain"**
- It will be something like: `frontier-back.up.railway.app`
- Your API is now live at: `https://frontier-back.up.railway.app`

---

## Step 5: Test the Backend

1. Visit: `https://frontier-back.up.railway.app/swagger` (Swagger UI)
2. Or: `https://frontier-back.up.railway.app/health` (if you have a health endpoint)
3. If you see a response, the backend is working!

---

## Step 6: Update Frontend CORS

Once your frontend is also deployed, update `FRONTEND_URL` in Railway:

1. Go to Railway Variables
2. Find `FRONTEND_URL`
3. Update it to: `https://your-frontend-domain.vercel.app` (or wherever it's deployed)
4. Railway will automatically restart the service

---

## Common Issues & Fixes

### ❌ "Connection refused" or database errors

- Check `DATABASE_URL` is correct in Railway Variables
- Verify Neon database exists and is running
- Test the connection string locally first

### ❌ "CORS error" on frontend

- Make sure `FRONTEND_URL` is set in Railway Variables
- Include the protocol (`https://` not just domain)
- Restart the Railway service after changing variables

### ❌ "Port already in use"

- Railway provides `PORT` env var automatically
- Code already reads it, so this shouldn't happen

### ❌ "Build failed"

- Check Dockerfile syntax
- Verify all project references are correct
- Check Railway build logs for details

---

## Useful Railway Commands (Optional)

If you want to use Railway CLI instead of GitHub:

```bash
# Install Railway CLI
npm install -g railway

# Login
railway login

# Initialize Railway project
cd frontier-back
railway init

# Deploy
railway up
```

---

## What Happens Now

1. **First deploy**: Railway builds the Docker image (takes ~5 min)
2. **Automatic rebuilds**: Every time you push to GitHub, Railway automatically rebuilds
3. **Database**: Neon Postgres is external, so your data persists
4. **Scale**: If you need more power, upgrade Railway plan

---

## Next: Deploy Frontend

Once backend is working, deploy your frontend to Vercel, Netlify, or Railway:

```bash
cd frontier-frontend
# Follow deployment instructions for your platform
```

Then update `FRONTEND_URL` in Railway backend variables to point to your deployed frontend.

---

## Your Current Setup

- **Backend**: Railway (deployed)
- **Database**: Neon Postgres (deployed)
- **Frontend**: (To be deployed separately)

All connected automatically via environment variables!
