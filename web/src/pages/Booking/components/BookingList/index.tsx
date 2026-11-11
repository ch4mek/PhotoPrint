import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';
import { Fragment, useCallback, useMemo, useState } from 'react';

import {
	BOOKING_LIST_LOADER_TEST_ID,
	BOOKING_LIST_TEST_ID,
} from '../../constants';

import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { scrollbarWidths } from '@/constants/commonStyles';
import {
	isManagerRoleOrGroupManager,
	isNotChiefRole,
	Roles,
} from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { useBookingContext } from '@/context/Booking';
import { SelectedBookingItemType } from '@/context/types';
import { isMeetingRoom } from '@/helpers/isMeetingRoom';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { DeleteBookingPopup } from '@/pages/Booking/components/PopUps/DeleteBookingPopup';
import { useDeleteDeskBooking } from '@/pages/Booking/hooks/useDeleteDeskBooking';
import { useDeleteMeetingRoomBooking } from '@/pages/Booking/hooks/useDeleteMeetingRoomBooking';
import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

import { DeskBookingListItem } from './DeskBookingListItem';
import { RoomBookingListItem } from './RoomBookingListItem';
import { ListWrapper } from './styles';
import { BookingListProps } from './types';
import { convertBookingItemToPopupData, getOrderedBookings } from './utils';

export function BookingList({
	deskData,
	roomsData,
	showLoader,
}: BookingListProps) {
	const { id, role } = useAuth();
	const {
		office,
		areMeetingRoomsSelected,
		bookingUsersData,
		itemToDelete,
		setItemToDelete,
	} = useBookingContext();

	const canOpenProfilePopup = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const { timeZone } = office;

	const [isDialogOpen, setIsDialogOpen] = useState(false);

	const { handleDeskBookingDeleting, isDeskBookingDeletingLoading } =
		useDeleteDeskBooking({ item: itemToDelete, id });

	const { handleRoomBookingDeleting, isRoomBookingDeletingLoading } =
		useDeleteMeetingRoomBooking({ item: itemToDelete, id });

	const sortedData = useMemo(
		() =>
			deskData && roomsData
				? getOrderedBookings(deskData, roomsData, areMeetingRoomsSelected)
				: [],
		[deskData, roomsData, areMeetingRoomsSelected]
	);

	const handleCloseDialog = useCallback(() => {
		setIsDialogOpen(false);
		setItemToDelete(null);
	}, []);

	const initiateBookDelete = useCallback(
		(item: DeskBooking | MeetingRoomBooking) => () => {
			setItemToDelete(item);
			setIsDialogOpen(true);
		},
		[setItemToDelete]
	);

	const handleDelete = useCallback(async () => {
		if (!itemToDelete) {
			return;
		}

		if (isMeetingRoom(itemToDelete)) {
			await handleRoomBookingDeleting({ entryId: itemToDelete.id });

			setItemToDelete(null);
			setIsDialogOpen(false);

			return;
		}

		const { bookingId } = itemToDelete;

		await handleDeskBookingDeleting({ id: bookingId });

		setItemToDelete(null);
		setIsDialogOpen(false);
	}, [itemToDelete]);

	const convertedItemToDelete = convertBookingItemToPopupData(itemToDelete);

	const handleClickName =
		(userId: string | number, isNeedOpenInfo: boolean = false) =>
		() => {
			if (!isNeedOpenInfo || !canOpenProfilePopup(+userId)) {
				return;
			}

			handlePageOpen(userId.toString());
		};

	const getIsNameClickable = useCallback(
		(
			bookingType: SelectedBookingItemType,
			userId: number,
			isForGuest?: boolean
		) => {
			const positionDescription = bookingUsersData?.find(
				(item) => item.id === userId
			)?.positionDescription as Roles;

			const hasProfilePopupAccess =
				canOpenProfilePopup(userId) &&
				isManagerRoleOrGroupManager(role) &&
				isNotChiefRole(positionDescription);

			if (bookingType === SelectedBookingItemType.Desk) {
				return !isForGuest && hasProfilePopupAccess;
			}

			if (bookingType === SelectedBookingItemType.Room) {
				return hasProfilePopupAccess;
			}

			return false;
		},
		[canOpenProfilePopup, role, bookingUsersData]
	);

	return (
		<>
			<ListWrapper>
				<Loader
					isLoading={showLoader}
					size="small"
					dataTestId={BOOKING_LIST_LOADER_TEST_ID}
				>
					<Scrollbar
						customScrollbarWidth={scrollbarWidths.s}
						scroll="vertical"
						maxHeight="100%"
						maxWidth="100%"
						data-testid={BOOKING_LIST_TEST_ID}
					>
						{sortedData.map(({ items, type }) =>
							type === SelectedBookingItemType.Room ? (
								<Fragment key={type}>
									{items.map((props) => (
										<RoomBookingListItem
											key={props.id}
											handleDelete={initiateBookDelete(props)}
											timeZone={timeZone}
											handleClickName={handleClickName}
											isNameClickable={getIsNameClickable(
												SelectedBookingItemType.Room,
												props.userId
											)}
											{...props}
										/>
									))}
								</Fragment>
							) : (
								<Fragment key={type}>
									{items.map((props) => (
										<DeskBookingListItem
											office={office}
											key={props.bookingId}
											handleDelete={initiateBookDelete(props)}
											currentUserId={id}
											userRole={role as Roles}
											handleClickName={handleClickName}
											isNameClickable={getIsNameClickable(
												SelectedBookingItemType.Desk,
												props.userId,
												props.isForGuest
											)}
											{...props}
										/>
									))}
								</Fragment>
							)
						)}
					</Scrollbar>
				</Loader>
			</ListWrapper>
			{isDialogOpen && convertedItemToDelete && (
				<DeleteBookingPopup
					isLoading={
						isDeskBookingDeletingLoading || isRoomBookingDeletingLoading
					}
					item={convertedItemToDelete}
					handleSubmit={handleDelete}
					handleClose={handleCloseDialog}
				/>
			)}
		</>
	);
}
