import json

categories = ['Food', 'Transport', 'Entertainment', 'Utilities']
expenses = [300, 150, 80, 200]

pie_chart_data = {
    "categories": categories,
    "expenses": expenses
}

print(json.dumps(pie_chart_data))
