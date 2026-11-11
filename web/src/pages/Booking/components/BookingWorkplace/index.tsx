import { memo } from 'react';

import { Loader } from '@/components/Loader';
import { MEETING_ROOMS_TAB_ID } from '@/constants/booking';
import { useBookingContext } from '@/context/Booking';
import { areMeetingRoomsOpen } from '@/helpers/bookingTime';
import { useFilledDesksWithRangeDate } from '@/hooks/useFilledDesksWithRangeDate';
import dayjs from '@/lib/dayjs';
import { useGetOfficeWorkplaces } from '@/queries/booking/useGetSpaceWorkplaces';

import { MeetingRoomsRenderer } from './components/MeetingRoomsRenderer';
import { SpacesTabs } from './components/SpacesTabs';
import { WorkplaceSpaceRenderer } from './components/WorkplaceSpaceRenderer';
import { useWorkplaceConfiguration } from './hooks/useWorkplaceConfiguration';
import { WorkplaceControls } from './Legend';
import { BookingWorkplaceWrapper, TitleWrapper } from './styled';
import { OfficeSpaceType } from './types';

function BookingWorkplaceInner() {
	const {
		firstDay,
		lastDay,
		error,
		office,
		setWorkplaces,
		selectedSpaceId,
		setSelectedSpaceId,
		resetOnMeetingTabChange,
		areMeetingRoomsSelected,
		officeWorkplaces,
	} = useBookingContext();

	const hasSpaces = (office.workspaces || []).length > 0;

	useFilledDesksWithRangeDate(
		dayjs(firstDay),
		dayjs(lastDay),
		setWorkplaces,
		error,
		office,
		selectedSpaceId || undefined,
		selectedSpaceId !== MEETING_ROOMS_TAB_ID
	);

	const { configuration, activeTabsId } = useWorkplaceConfiguration({
		office,
		spaceId: selectedSpaceId,
		officeWorkplaces,
	});

	const handleSpacesTabChange = (spaceId: number) => {
		if (areMeetingRoomsSelected || areMeetingRoomsOpen(spaceId)) {
			const officeDate = dayjs().tz(office.timeZone);
			resetOnMeetingTabChange(officeDate);
		}

		setSelectedSpaceId(spaceId);
	};

	const { isLoading } = useGetOfficeWorkplaces(office.id);

	const isLoaderShown = isLoading || configuration === null;

	return (
		<Loader isLoading={isLoaderShown}>
			<BookingWorkplaceWrapper>
				{hasSpaces && selectedSpaceId && (
					<TitleWrapper>
						<div>
							<SpacesTabs
								hasMeetingRooms={office.meetingRooms.length > 0}
								activeTab={selectedSpaceId}
								onChange={handleSpacesTabChange}
								spaces={office.workspaces}
								activeTabsId={activeTabsId}
							/>
						</div>
					</TitleWrapper>
				)}
				{configuration?.type === OfficeSpaceType.Space && (
					<>
						<WorkplaceSpaceRenderer
							config={configuration}
							spaceId={selectedSpaceId}
						/>
						<WorkplaceControls showLegend />
					</>
				)}
				{configuration?.type === OfficeSpaceType.MeetingRooms && (
					<>
						<MeetingRoomsRenderer />
						<WorkplaceControls showLegend={false} />
					</>
				)}
			</BookingWorkplaceWrapper>
		</Loader>
	);
}

export const BookingWorkplace = memo(BookingWorkplaceInner);
