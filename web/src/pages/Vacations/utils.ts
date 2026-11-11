import { EmployeeVacations, LeaveDay } from './types';

export const darkenColor = (hex: string, percent: number): string => {
	const num = parseInt(hex.slice(1), 16);
	const r = Math.max(0, Math.floor(num / 0x10000) - (255 * percent) / 100);
	const g = Math.max(
		0,
		Math.floor((num / 0x100) % 0x100) - (255 * percent) / 100
	);
	const b = Math.max(0, Math.floor(num % 0x100) - (255 * percent) / 100);

	return `rgb(${Math.round(r)}, ${Math.round(g)}, ${Math.round(b)})`;
};

const parseDate = (dateStr: string): Date => {
	const [day, month, year] = dateStr.split('.').map(Number);

	return new Date(year, month - 1, day);
};

export const groupByMonth = (employees: EmployeeVacations[]) =>
	employees.reduce(
		(monthsMap: { [key: string]: EmployeeVacations[] }, employee) =>
			employee.leaveDays.reduce(
				(acc, leave: LeaveDay) => {
					const date = parseDate(leave.date);
					const monthName = new Intl.DateTimeFormat('en-US', {
						month: 'long',
					}).format(date);
					const monthKey = `${date.getFullYear()}, ${monthName}`;

					if (!acc[monthKey]) {
						acc[monthKey] = [];
					}

					const existingEmployee = acc[monthKey].find(
						(e) => e.id === employee.id
					);

					if (existingEmployee) {
						existingEmployee.leaveDays.push(leave);
					} else {
						acc[monthKey].push({
							...employee,
							leaveDays: [leave],
						});
					}

					return acc;
				},
				{ ...monthsMap }
			),
		{}
	);

export const getMinDate = (employees: EmployeeVacations[]) => {
	const allDates = employees
		.map((employee) => employee.leaveDays)
		.flat()
		.map((leave) => parseDate(leave.date));

	return new Date(Math.min(...allDates.map((date) => date.getTime())));
};

export const aggregateEmployeeLeaveDays = (employees: EmployeeVacations[]) =>
	employees.reduce((aggregatedEmployees: EmployeeVacations[], employee) => {
		const existingEmployee = aggregatedEmployees.find(
			(e) => e.id === employee.id
		);

		if (existingEmployee) {
			existingEmployee.leaveDays = [
				...existingEmployee.leaveDays,
				...employee.leaveDays,
			];
		} else {
			aggregatedEmployees.push({ ...employee });
		}

		return aggregatedEmployees;
	}, []);
