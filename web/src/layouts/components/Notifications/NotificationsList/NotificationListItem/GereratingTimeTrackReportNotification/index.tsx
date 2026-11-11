import { Typography } from '@mms/mms-ui-library/components/Typography';

import { REPORT_IS_BEING_GENERATED } from '@/constants/staticText';

import { TimeTrackReportNotificationBody } from './styles';

export function GeneratingTimeTrackReportNotification() {
	return (
		<TimeTrackReportNotificationBody>
			<Typography variant="m-400">{REPORT_IS_BEING_GENERATED}</Typography>
		</TimeTrackReportNotificationBody>
	);
}
