import { EMPLOYEE_CONTRACT_EXPIRES_IN } from '@/constants/notifications';
import { EmployeeContractExpiringNotification } from '@/types/notifications';

export const generateNotificationText = (
	item: EmployeeContractExpiringNotification
) => {
	const { UserFirstName, UserLastName, DaysLeft } = item;

	return `${UserLastName} ${UserFirstName} contract expires in ${EMPLOYEE_CONTRACT_EXPIRES_IN[DaysLeft]}.`;
};
