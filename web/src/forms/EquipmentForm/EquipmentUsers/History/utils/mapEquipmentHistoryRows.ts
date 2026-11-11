import { formatDateString } from '@/helpers/dateToISOString';
import { ComplexConfigurableConfigurationItem } from '@/types/configuration';
import { EquipmentHistoryItem } from '@/types/equipment';

export const mapEquipmentHistoryRows = (
	equipmentHistory: EquipmentHistoryItem[],
	departmentTypes: ComplexConfigurableConfigurationItem[]
) =>
	equipmentHistory.map(
		({
			id,
			departmentId,
			issueDate,
			returnDate,
			equipmentHistoryId,
			approverFirstName,
			approverLastName,
			firstName,
			lastName,
		}) => {
			const department = departmentTypes.find(
				(dep) => dep.id === departmentId
			)?.fullName;

			return {
				id,
				userName: {
					firstName,
					lastName,
				},
				assignmentApprover: {
					firstName: approverFirstName,
					lastName: approverLastName,
				},
				departmentId: department,
				issueDate: formatDateString(issueDate),
				returnDate: returnDate ? formatDateString(returnDate) : '',
				equipmentHistoryId,
				showSkeleton: false,
			};
		}
	);
