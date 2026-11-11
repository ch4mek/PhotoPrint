import { Typography } from '@mms/mms-ui-library/components/Typography';
import { WarningErrorIcon } from '@mms/mms-ui-library/icons/base/WarningErrorIcon';

import { REPORT_GENERATING_FAILED } from '@/constants/errors';

import { ReportGeneratingFailureNotificationBody } from './styles';

export function ReportGeneratingFailureNotification() {
	return (
		<ReportGeneratingFailureNotificationBody>
			<WarningErrorIcon />
			<Typography variant="m-400">{REPORT_GENERATING_FAILED}</Typography>
		</ReportGeneratingFailureNotificationBody>
	);
}
