import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useIsFetching } from '@tanstack/react-query';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BookingWarningPopup } from '../../../BookingWarningPopup';
import { PopUpBody } from '../../../PopUps/PopUpBody';
import { SelectedItemView } from '../SelectedSeatView';

import { ProtectedView } from '@/components/ProtectedView';
import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { chiefRoles, managerRolesWithGroupManager } from '@/constants/roles';
import { useBookingContext } from '@/context/Booking';
import { useAuth } from '@/context/index';
import { SelectedBookingItemType } from '@/context/types';
import {
	getDeskBlockDialogSubtext,
	getTooltipTitle,
	isAllowToBook,
} from '@/helpers/booking';
import { Dayjs } from '@/lib/dayjs';
import { getDeskName } from '@/pages/Booking/utils';

import { AllDaySwitch } from './AllDaySwitch';
import { BookButton } from './BookButton';
import { useMutateBookingItem } from './hooks/useMutateBookingItem';
import { Repetitive } from './Repetitive';
import { ButtonsWrapper, Wrapper, SwitchContainer } from './styles';
import { getBookingItemName } from './utils';

export function BookingControls() {
	const [isDialogOpen, setIsDialogOpen] = useState(false);

	const [isChecked, setIsChecked] = useState(false);
	const [warning, setWarning] = useState<null | string>(null);
	const { role, departmentId, email } = useAuth();
	const { t } = useTranslation('booking');

	const { t: tPlaceholders } = useTranslation('common', {
		keyPrefix: 'placeholders',
	});

	const { t: tCommon } = useTranslation('common', { keyPrefix: 'modals' });

	const areCollisionsLoading = useIsFetching({
		queryKey: [BookingQueriesKeys.collisions],
	});

	const {
		selectedItem,
		areMeetingRoomsSelected,
		firstDay,
		lastDay,
		office,
		setWeeksCount,
		weeksCount,
		isAllDayChecked,
		setIsAllDayChecked,
		hasWorkplaceCollision,
		areMeetingRoomDisabled,
		isBlockedDesk,
		userHasRoomBookingsForTimelinePeriod,
		isSelectedMeetingRoomTimeInPast,
		isGuestMode,
		guestNameError,
		guestName,
		error: formError,
	} = useBookingContext();

	useEffect(() => {
		setWeeksCount(isChecked ? 1 : 0);
	}, [isChecked]);

	const handleChange = useCallback(() => {
		setWeeksCount(isChecked ? 1 : 0);
		setIsChecked((isPrevChecked) => !isPrevChecked);
	}, [isChecked]);

	const handleAllDayChange = useCallback(() => {
		setIsAllDayChecked(!isAllDayChecked);
	}, [isAllDayChecked]);

	const handleWeeksChange = useCallback((value: number) => {
		setWeeksCount(value);
	}, []);

	const areButtonsDisabled = selectedItem === null || !firstDay || !lastDay;
	const areBookButtonGuestModeDisabled =
		isGuestMode && !areMeetingRoomsSelected
			? !!guestNameError || !guestName
			: false;

	const handleClose = () => setWarning(null);

	const initiateDeskBlock = useCallback(() => {
		if (areButtonsDisabled) {
			return;
		}
		setIsDialogOpen(true);
	}, [areButtonsDisabled]);

	const handleCloseDialog = useCallback(() => {
		setIsDialogOpen(false);
	}, []);

	const { handleBookClick, handleBlock } = useMutateBookingItem(
		isChecked,
		setIsChecked,
		setIsDialogOpen,
		setWarning
	);

	const isSelectedDateSingle = firstDay?.isSame(lastDay, 'day') || false;
	const isRepetitiveDisabled = !isSelectedDateSingle || Boolean(formError);

	useEffect(() => {
		setIsChecked(false);
	}, [
		office,
		isSelectedDateSingle,
		firstDay,
		areMeetingRoomsSelected,
		formError,
	]);

	const allowToBookSelectedWorkplace = useMemo(
		() =>
			isAllowToBook(
				role,
				departmentId,
				office,
				selectedItem?.type === SelectedBookingItemType.Desk
					? selectedItem.spaceId
					: null,
				email
			),
		[role, departmentId, office, selectedItem, email]
	);

	const commonDisableConditions =
		Boolean(formError) || !allowToBookSelectedWorkplace || areButtonsDisabled;

	const isBookButtonDisabled =
		commonDisableConditions ||
		(hasWorkplaceCollision === undefined && Boolean(areCollisionsLoading)) ||
		areMeetingRoomDisabled ||
		areBookButtonGuestModeDisabled ||
		(!areMeetingRoomsSelected && hasWorkplaceCollision) ||
		(areMeetingRoomsSelected &&
			(isSelectedMeetingRoomTimeInPast ||
				userHasRoomBookingsForTimelinePeriod));

	const isBlockDisabled =
		isBookButtonDisabled ||
		commonDisableConditions ||
		isGuestMode ||
		isBlockedDesk ||
		!isSelectedDateSingle;

	return (
		<>
			<Wrapper>
				<div>
					<SwitchContainer isTwoRows={isChecked}>
						<Repetitive
							isChecked={isChecked}
							isControlDisabled={isRepetitiveDisabled}
							weeksCount={weeksCount}
							handleWeeksChange={handleWeeksChange}
							handleChange={handleChange}
						/>
						{areMeetingRoomsSelected && (
							<ProtectedView roles={chiefRoles}>
								<AllDaySwitch
									checked={isAllDayChecked}
									isDisabled={false}
									handleChange={handleAllDayChange}
								/>
							</ProtectedView>
						)}
					</SwitchContainer>
					{selectedItem && <SelectedItemView item={selectedItem} />}
				</div>
				<ButtonsWrapper isChiefRole={chiefRoles.includes(role)}>
					<BookButton
						disabled={isBookButtonDisabled}
						tooltipTitle={selectedItem ? t('tooltips.bookingNotAllowed') : ''}
						onClick={handleBookClick}
						color="primary"
					>
						{getBookingItemName(areMeetingRoomsSelected, t)}
					</BookButton>
					{!areMeetingRoomsSelected && (
						<ProtectedView roles={managerRolesWithGroupManager}>
							<BookButton
								disabled={isBlockDisabled}
								color="secondary"
								tooltipTitle={
									getTooltipTitle(
										lastDay as Dayjs,
										selectedItem,
										areMeetingRoomsSelected,
										true,
										t
									) || ''
								}
								onClick={initiateDeskBlock}
							>
								{t('actions.blockDesk')}
							</BookButton>
						</ProtectedView>
					)}
				</ButtonsWrapper>
			</Wrapper>
			{warning && (
				<BookingWarningPopup handleClose={handleClose} warning={warning} />
			)}
			{!areButtonsDisabled &&
				isDialogOpen &&
				selectedItem.type === SelectedBookingItemType.Desk && (
					<PopUp
						title={t('dialogs.blockDesk.header')}
						onClose={handleCloseDialog}
						type="approve"
						showCloseButton={false}
						controls={{
							negativeControl: {
								onClick: handleCloseDialog,
								text: tCommon('controls.no'),
							},
							positiveControl: {
								onClick: handleBlock,
								text: tCommon('controls.yes'),
							},
						}}
					>
						<PopUpBody>
							<Typography variant="m-400">
								{t('dialogs.blockDesk.confirm')}
							</Typography>
							<Typography variant="m-600">
								{getDeskName(
									selectedItem.abbreviation,
									selectedItem.displayNumber
								)}
							</Typography>
							<Typography variant="m-600">
								{getDeskBlockDialogSubtext(firstDay, tPlaceholders)}
							</Typography>
						</PopUpBody>
					</PopUp>
				)}
		</>
	);
}
