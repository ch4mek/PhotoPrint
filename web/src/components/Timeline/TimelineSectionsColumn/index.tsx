import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	TimelineSectionsColumnTitleWrapper,
	TimelineSectionsColumnWrapper,
} from './styles';
import { TimelineSection } from './TimelineSection';
import { TimelineSectionsColumnProps } from './types';

function TimelineSectionsColumnInner({
	sections,
	selectedSectionId,
	onSectionSelection,
}: TimelineSectionsColumnProps) {
	const theme = useTheme();
	const { t } = useTranslation('booking', { keyPrefix: 'spaces' });

	return (
		<TimelineSectionsColumnWrapper>
			<TimelineSectionsColumnTitleWrapper>
				<Typography color={theme.palette['text-black']} variant="m-400">
					{t('rooms')}
				</Typography>
			</TimelineSectionsColumnTitleWrapper>
			<div>
				{sections.map(
					({ meetingRoomId, meetingRoomName, equipments, countOfSeats }) => (
						<TimelineSection
							key={meetingRoomId}
							meetingRoomId={meetingRoomId}
							meetingRoomName={meetingRoomName}
							isSelected={selectedSectionId === meetingRoomId}
							onSectionSelection={onSectionSelection}
							equipments={equipments}
							countOfSeats={countOfSeats}
						/>
					)
				)}
			</div>
		</TimelineSectionsColumnWrapper>
	);
}

export const TimelineSectionsColumn = memo(TimelineSectionsColumnInner);
