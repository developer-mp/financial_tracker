import matplotlib.pyplot as plt

# Sample data, replace this with your actual data
categories = ['Category A', 'Category B', 'Category C']
expenses = [1000, 2000, 1500]

# Create the pie chart
plt.figure(figsize=(8, 8))
plt.pie(expenses, labels=categories, autopct='%1.1f%%', startangle=140)
plt.axis('equal')

# Save the pie chart as an image file
plt.savefig('pie_chart.png')

# Close the figure to free up resources
plt.close()
