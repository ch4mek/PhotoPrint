import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import Backdrop from '@mui/material/Backdrop';
import { useCallback } from 'react';

import { DeskInfo } from '../../types';

import { isManagerRoleOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useBookingContext } from '@/context/Booking';
import { SelectedBookingItemType } from '@/context/types';
import { convertBookingItemToPopupData } from '@/pages/Booking/components/BookingList/utils';
import { DeleteBookingPopup } from '@/pages/Booking/components/PopUps/DeleteBookingPopup';
import { WORKPLACE_ELEMENT_ID } from '@/pages/Booking/constants';
import { useDeleteDeskBooking } from '@/pages/Booking/hooks/useDeleteDeskBooking';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { DeskBooking } from '@/types/booking';

import { Desk } from './components/Desk';
import { SpaceLayoutWrapper, WorkplaceWrapper } from './styled';
import { WorkplaceSpaceRendererProps } from './types';

export function WorkplaceSpaceRenderer(props: WorkplaceSpaceRendererProps) {
	const { config, spaceId } = props;
	const { seats, spaceAbbreviation, SpaceLayout, deskSize } = config;
	const { role, id } = useAuth();
	const { handleClose, handleOpen, isOpen } = useShowModal();

	const {
		itemToDelete,
		setItemToDelete,
		selectedItem,
		setSelectedItem,
		workplaces,
		lastDay,
		error,
		setHasWorkplaceCollision,
		bookingsState,
		office,
	} = useBookingContext();

	const { handleDeskBookingDeleting, isDeskBookingDeletingLoading } =
		useDeleteDeskBooking({ item: itemToDelete, id });

	const handleClick = useCallback(
		(item: DeskBooking | null) => (deskInfo: DeskInfo | null) => {
			setItemToDelete(item);

			if (deskInfo) {
				setSelectedItem({
					...deskInfo,
					spaceId,
					abbreviation: spaceAbbreviation,
					type: SelectedBookingItemType.Desk,
				});
			} else {
				setSelectedItem(null);
				setHasWorkplaceCollision(undefined);
			}
		},
		[lastDay, spaceId, spaceAbbreviation]
	);

	const convertedItemToDelete = convertBookingItemToPopupData(itemToDelete);
	const isDesktop = useHasScreenWidth(breakpoints.tablet);

	const handleCloseDeletingPopup = useCallback(() => {
		handleClose();
		setSelectedItem(null);
	}, []);

	const handleBookingDeleting = useCallback(async () => {
		if (!itemToDelete) {
			return;
		}

		const deskItem = itemToDelete as DeskBooking;

		await handleDeskBookingDeleting({ id: deskItem.bookingId });
		handleCloseDeletingPopup();
	}, [itemToDelete]);

	return (
		<>
			<WorkplaceWrapper scroll="both" maxHeight="100%" maxWidth="100%">
				<SpaceLayoutWrapper>
					<SpaceLayout key={spaceAbbreviation} id={WORKPLACE_ELEMENT_ID}>
						{seats.map((seat) => {
							const workplace = workplaces.find(
								({ workplaceId }) => workplaceId === seat.id
							);

							const booking = bookingsState.desks?.bookings.find(
								({ workplaceId: workplaceBookingId, bookingId }) =>
									workplace?.workplaceId === workplaceBookingId &&
									bookingId === workplace?.bookingId
							);
							const isCanCanceled =
								!!booking &&
								!(workplace?.isBlocked && !isManagerRoleOrGroupManager(role));

							return (
								<Desk
									{...seat}
									{...workplace}
									{...{ isCanCanceled, office }}
									size={deskSize}
									isMobileView={isDesktop}
									key={seat.id}
									onClick={handleClick(booking || null)}
									isSelected={seat.id === selectedItem?.id}
									dateList={booking?.dateList}
									weeksInInterval={booking?.weeksInInterval}
									workspaceId={booking?.workspaceId || null}
									handleCancelClick={handleOpen}
								/>
							);
						})}
						{error && <Backdrop sx={{ position: 'absolute' }} open invisible />}
					</SpaceLayout>
				</SpaceLayoutWrapper>
			</WorkplaceWrapper>
			{isOpen && convertedItemToDelete && (
				<DeleteBookingPopup
					isLoading={isDeskBookingDeletingLoading}
					item={convertedItemToDelete}
					handleSubmit={handleBookingDeleting}
					handleClose={handleCloseDeletingPopup}
				/>
			)}
		</>
	);
}
