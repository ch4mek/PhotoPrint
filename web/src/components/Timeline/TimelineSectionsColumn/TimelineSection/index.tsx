import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

import { CELL_HEIGHT } from '../../constants';
import { useTimelineContext } from '../../TimelineContext';
import { MeetingRoomEquipmentTypes } from '../../types';

import { useResizeObserver } from '@/hooks/useResizeObserver';

import { SectionEquipmentItem } from './components/SectionEquipmentItem';
import {
	MEETING_ROOM_EQUIPMENT_LABEL_KEYS,
	MeetingRoomEquipment,
	MIN_SECTION_HEIGHT,
	ROOM_NUMBER_SIGN,
} from './constants';
import {
	MeetingRoomNameTypography,
	SectionEquipment,
	TimelineSectionContent,
	TimelineSectionSkeleton,
	TimelineSectionWrapper,
} from './styles';
import { TimelineSectionProps } from './types';

export function TimelineSection({
	meetingRoomId,
	meetingRoomName,
	isSelected,
	onSectionSelection,
	equipments,
	countOfSeats,
}: TimelineSectionProps) {
	const { t } = useTranslation('booking');
	const {
		showLoader,
		isPeriodSelectionDisabled,
		timelineSectionsHeight,
		setTimelineSectionsHeight,
	} = useTimelineContext();

	const getEquipmentLabel = (type: MeetingRoomEquipmentTypes) =>
		t(MEETING_ROOM_EQUIPMENT_LABEL_KEYS[type]);

	const onTimelineSectionClick = () => {
		if (
			typeof onSectionSelection === 'function' &&
			!showLoader &&
			!isPeriodSelectionDisabled
		) {
			onSectionSelection({
				meetingRoomId,
				meetingRoomName,
				equipments,
				countOfSeats,
			});
		}
	};
	const { ref, height } = useResizeObserver();

	useEffect(() => {
		if (height > MIN_SECTION_HEIGHT) {
			setTimelineSectionsHeight((prev) => ({
				...prev,
				[meetingRoomId]: height > CELL_HEIGHT ? height : CELL_HEIGHT,
			}));
		}
	}, [height, meetingRoomId, setTimelineSectionsHeight]);

	return (
		<TimelineSectionWrapper
			ref={ref}
			timelineHeight={timelineSectionsHeight[meetingRoomId]}
			onClick={onTimelineSectionClick}
			isDisabled={showLoader || isPeriodSelectionDisabled}
			isSelected={isSelected && !showLoader && !isPeriodSelectionDisabled}
		>
			{!showLoader ? (
				<TimelineSectionContent>
					<MeetingRoomNameTypography variant="m-600">
						{ROOM_NUMBER_SIGN} {meetingRoomName}
					</MeetingRoomNameTypography>
					<SectionEquipment>
						<SectionEquipmentItem
							count={countOfSeats}
							icon={MeetingRoomEquipment[MeetingRoomEquipmentTypes.SEATS].icon}
							name={getEquipmentLabel(MeetingRoomEquipmentTypes.SEATS)}
						/>
						{equipments.map(({ id, count, type }) => (
							<SectionEquipmentItem
								key={id}
								count={count}
								icon={MeetingRoomEquipment[type].icon}
								name={getEquipmentLabel(type)}
							/>
						))}
					</SectionEquipment>
				</TimelineSectionContent>
			) : (
				<TimelineSectionSkeleton animation="wave" />
			)}
		</TimelineSectionWrapper>
	);
}
