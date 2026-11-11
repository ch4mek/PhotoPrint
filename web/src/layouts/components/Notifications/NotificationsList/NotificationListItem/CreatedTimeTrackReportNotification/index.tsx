import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback, useContext } from 'react';

import { NotificationsParameteresContext } from '../../context/NotificationsContext';
import { getFileTypeIcon } from '../utils';

import { EXPIRED_REPORT_LINK_TOAST } from '@/constants/errors';
import { NotificationType as ToastNotificationType } from '@/constants/notifications';
import { REPORT_IS_GENERATED, REPORT_SIZE_UNIT } from '@/constants/staticText';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';
import { useGetReportFile } from '@/queries/notifications/useGetReportFile';
import { TimeTrackReportCreatedNotification } from '@/types/notifications';

import {
	TimeTrackReportFileDescriptionContainer,
	TimeTrackReportNotificationBody,
	TimeTrackReportNotificationInfoContainer,
} from './styles';

interface TimeTrackReportNotificationProps {
	item: TimeTrackReportCreatedNotification;
}

export function CreatedTimeTrackReportNotification({
	item,
}: TimeTrackReportNotificationProps) {
	const { ReportName, DocumentType, id, SizeInKilobytes, ReportFileId, read } =
		item;
	const FileTypeIcon = getFileTypeIcon(DocumentType);
	const toast = useToast();

	const { refetch: startDownloadingFile } = useGetReportFile(ReportFileId, id, {
		enabled: false,
		onSuccess: (responseData) => {
			triggerFileDownload(responseData, ReportName);
		},
		onError: () => {
			toast.open(
				createToast(ToastNotificationType.ERROR, EXPIRED_REPORT_LINK_TOAST)
			);
		},
	});

	const { readNotification } = useContext(NotificationsParameteresContext);

	const handleNotificationBodyClick = useCallback(() => {
		if (!read) {
			readNotification(id);
		}

		startDownloadingFile();
	}, [read]);

	return (
		<TimeTrackReportNotificationBody onClick={handleNotificationBodyClick}>
			<Typography variant="m-400">{REPORT_IS_GENERATED}</Typography>
			<TimeTrackReportNotificationInfoContainer>
				<FileTypeIcon />
				<TimeTrackReportFileDescriptionContainer>
					<Typography variant="s-600" fullWidth>
						{ReportName}
					</Typography>
					<Typography variant="s-400">
						{SizeInKilobytes} {REPORT_SIZE_UNIT}
					</Typography>
				</TimeTrackReportFileDescriptionContainer>
			</TimeTrackReportNotificationInfoContainer>
		</TimeTrackReportNotificationBody>
	);
}
