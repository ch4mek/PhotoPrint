import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useCallback, memo, useMemo, useRef } from 'react';
import { useTranslation } from 'react-i18next';

import { getPortalContainer } from '@/components/Grid/utils/getPortalContainer';
import { BOOKING_ITEM_TIME_FORMAT_WITH_COMMA } from '@/constants/booking';
import { useAuth } from '@/context/AuthContext';
import { createPeriodInText } from '@/helpers/booking';
import { formatDisplayDate } from '@/helpers/bookingFormatters';
import { getFullName } from '@/helpers/formatName';
import { useOpenPersonalInfo } from '@/hooks/useOpenPersonalInfo';
import { CancelBookingPopover } from '@/pages/Booking/components/PopUps/CancelBookingPopover';
import { WORKPLACE_ELEMENT_ID } from '@/pages/Booking/constants';
import { CancelType } from '@/pages/Booking/types';
import { getDeskName, getSpaceAbbreviation } from '@/pages/Booking/utils';
import { useShowModal } from '@/pages/hooks/useShowModal';

import {
	DESK_TEST_ID,
	deskTooltipFallbackPlacements,
	GUEST_LAST_NAME_FILLER_TEXT,
} from './constants';
import {
	WorkplaceContent,
	DeskWrapper,
	DeskTitle,
	BookedAvatar,
	DeskTooltipTextWrapper,
	DeskSeatWrapper,
	DeskSeatContentWrapper,
} from './styled';
import type { DeskInnerProps } from './types';
import { getDeskSizes } from './utils';

const { Booking, Blocking } = CancelType;

function DeskInner({
	id,
	top,
	left,
	firstName,
	lastName,
	userId,
	isBlocked,
	isSelected,
	onClick,
	rotationDegree,
	displayNumber,
	isMobileView,
	size,
	isDisabled,
	isForGuest,
	guestName,
	bookedBy,
	rotate,
	dateList,
	weeksInInterval,
	workspaceId,
	workplaceNumber,
	office,
	isCanCanceled,
	handleCancelClick,
}: DeskInnerProps) {
	const deskRef = useRef(null);
	const { handleClose, handleOpen, isOpen } = useShowModal();
	const { t } = useTranslation('common');
	const { t: tBooking } = useTranslation('booking');

	const { id: personId } = useAuth();

	const { handleClickName } = useOpenPersonalInfo({
		userId,
		onClose: handleClose,
		isGuest: isForGuest,
	});

	const userInitials =
		lastName && firstName && getFullName(firstName, lastName);

	const isBooked = !isBlocked && !!userId && userId !== personId;
	const isBookedByUser = userId === personId;

	const handleClick = useCallback(() => {
		const isCancelPopupVisible = !isDisabled && isCanCanceled;

		onClick(
			isSelected || isDisabled
				? null
				: {
						id,
						displayNumber,
						isCanCanceled: isCancelPopupVisible,
				  }
		);

		if (isCancelPopupVisible) {
			handleOpen();
		}
	}, [onClick, isSelected, isDisabled, isCanCanceled]);

	const deskSizes = useMemo(
		() => getDeskSizes(size, rotationDegree),
		[size, rotationDegree]
	);

	const bookedDate = useMemo(() => {
		if (!dateList) {
			return null;
		}

		if (isBlocked) {
			const formattedDate = formatDisplayDate(
				dateList[0],
				BOOKING_ITEM_TIME_FORMAT_WITH_COMMA
			);

			return `${t('placeholders.from')} ${formattedDate}`;
		}

		return createPeriodInText(weeksInInterval || 0, dateList, t);
	}, [isBlocked, dateList, weeksInInterval]);

	const spaceInfo = useMemo(() => {
		if (!(workplaceNumber && office.id)) {
			return '';
		}

		return getDeskName(
			getSpaceAbbreviation(office.id, workspaceId),
			workplaceNumber
		);
	}, [office, workspaceId, workplaceNumber]);

	const handleSubmit = useCallback(() => {
		handleClose();
		handleCancelClick();
	}, []);

	return (
		<>
			<Tooltip
				open={Boolean(userInitials)}
				position="right"
				header={
					<DeskTooltipTextWrapper isMobile={Boolean(isMobileView)}>
						{isBlocked
							? tBooking('tooltips.deskBlockedBy')
							: tBooking('tooltips.deskBookedBy')}{' '}
						{isForGuest
							? `${bookedBy} ${t(
									'placeholders.for'
							  ).toLowerCase()} ${guestName}`
							: userInitials}
					</DeskTooltipTextWrapper>
				}
				getPortalContainer={getPortalContainer(`#${WORKPLACE_ELEMENT_ID}`)}
				fallbackPlacements={deskTooltipFallbackPlacements}
			>
				<WorkplaceContent
					{...{ size, left, top, rotate }}
					onClick={handleClick}
					data-testid={DESK_TEST_ID}
				>
					<DeskSeatContentWrapper rotationDegree={rotationDegree} ref={deskRef}>
						<DeskSeatWrapper
							{...{ rotationDegree, rotate }}
							isDisabled={Boolean(isDisabled)}
						/>
						<DeskWrapper
							{...deskSizes}
							{...{
								isBooked,
								isBookedByUser,
								rotationDegree,
								rotate,
								isDisabled,
							}}
							isForGuest={!!isForGuest}
							isBlocked={Boolean(isBlocked)}
							isSelected={Boolean(isSelected)}
						>
							<DeskTitle {...{ rotationDegree }} Component="h6" variant="s-600">
								<>
									{displayNumber}
									{userInitials && (
										<BookedAvatar
											firstName={
												isForGuest ? tBooking('labels.guest') : firstName
											}
											lastName={
												isForGuest ? GUEST_LAST_NAME_FILLER_TEXT : lastName
											}
										/>
									)}
								</>
							</DeskTitle>
						</DeskWrapper>
					</DeskSeatContentWrapper>
				</WorkplaceContent>
			</Tooltip>

			{isOpen && !!isSelected && (
				<CancelBookingPopover
					{...{
						handleClose,
						bookedDate,
						spaceInfo,
						handleSubmit,
					}}
					type={isBlocked ? Blocking : Booking}
					ref={deskRef}
					isOpen={isOpen && isSelected}
					bookedUserInfo={isForGuest ? guestName : userInitials}
					handleInfoClick={handleClickName}
				/>
			)}
		</>
	);
}

export const Desk = memo(DeskInner);
