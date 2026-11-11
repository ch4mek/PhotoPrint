import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	ALLOWED_FILE_FORMAT_TITLE,
	MAX_UPLOAD_FILE_SIZE_TITLE,
} from './constants';
import { NotificationTitle } from './styles';
import type { ProfileNotificationProps } from './types';

export function ProfileNotification({
	allowedFormats,
	maximumFileSizeInMb,
}: ProfileNotificationProps) {
	const maxFileSizeText = `${maximumFileSizeInMb} Mb.`;

	return (
		<>
			<Typography Component="p" variant="s-400">
				<NotificationTitle variant="m-600">
					{MAX_UPLOAD_FILE_SIZE_TITLE}
				</NotificationTitle>
				{maxFileSizeText}
			</Typography>
			<Typography Component="p" variant="s-400">
				<NotificationTitle variant="m-600">
					{ALLOWED_FILE_FORMAT_TITLE}
				</NotificationTitle>
				{allowedFormats}.
			</Typography>
		</>
	);
}
