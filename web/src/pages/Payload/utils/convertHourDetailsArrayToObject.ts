import { PERIOD } from '@/constants/date';
import { EmployeeProjects, HoursDetail } from '@/pages/Payload/types';

export function convertHourDetailsArrayToObject(
	monthArray: HoursDetail[],
	project: EmployeeProjects[]
) {
	return PERIOD.reduce((acc, month, index) => {
		const hourDetailForMonth = monthArray.find(
			(item) => item.monthOfYear === index + 1
		);

		const projectDetails = hourDetailForMonth
			? hourDetailForMonth.projectDetails.map((details) => ({ ...details }))
			: [];

		if (projectDetails.length < project.length) {
			project.forEach(({ projectId }) => {
				if (!projectDetails.some((proj) => proj.projectId === projectId)) {
					projectDetails.push({
						projectId,
						actualFte: 0,
						plannedFte: 0,
						plannedWorkloadInHours: 0,
						requiredHours: 0,
						trackedBillableInHours: 0,
					});
				}
			});
		}

		acc[month.toLowerCase()] = hourDetailForMonth
			? {
					...hourDetailForMonth,
					projectDetails: [...projectDetails],
			  }
			: {
					hasActivity: false,
					monthOfYear: index + 1,
					actualFte: 0,
					plannedFte: 0,
					plannedWorkloadInHours: 0,
					trackedBillableInHours: 0,
					requiredHours: 0,
					projectDetails: [],
			  };

		return acc;
	}, {} as Record<string, HoursDetail>);
}
