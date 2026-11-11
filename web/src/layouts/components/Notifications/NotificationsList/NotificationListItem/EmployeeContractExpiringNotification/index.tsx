import { Typography } from '@mms/mms-ui-library/components/Typography';

import { EmployeeContractExpiringNotificationBody } from './styles';
import { EmployeeContractExpiringNotificationProps } from './types';
import { generateNotificationText } from './utils';

export function EmployeeContractExpiringNotification({
	item,
}: EmployeeContractExpiringNotificationProps) {
	return (
		<EmployeeContractExpiringNotificationBody>
			<Typography variant="m-400">{generateNotificationText(item)}</Typography>
		</EmployeeContractExpiringNotificationBody>
	);
}
