import { useTheme } from '@emotion/react';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { CalendarIcon } from '@mms/mms-ui-library/icons/base/CalendarIcon';
import { ClockIcon } from '@mms/mms-ui-library/icons/base/ClockIcon';
import { MapPinIcon } from '@mms/mms-ui-library/icons/base/MapPinIcon';
import { RefObject, useMemo, useRef } from 'react';

import { isRefObject } from '@/helpers/isRefObject';
import { CancelButton } from '@/pages/Booking/components/PopUps/CancelBookingPopover/components/CancelButton';
import {
	CancelBookingPopoverWrapper,
	CancelContentWrapper,
	ClickableRowContentTypography,
	getCancelBookingPopoverStyles,
	IconWrapper,
	ItemRowWithIcon,
	RowContentTypography,
} from '@/pages/Booking/components/PopUps/CancelBookingPopover/styles';

import { CancelBookingPopoverProps } from './types';

export function CancelBookingPopover({
	handleSubmit,
	handleClose,
	onWrapperClick,
	isOpen,
	type,
	spaceInfo,
	bookedDate,
	handleInfoClick,
	bookedUserInfo = '',
	bookedDateTime,
	distanceToAnchorElement = 4,
	ref,
}: CancelBookingPopoverProps) {
	const theme = useTheme();
	const cancelPopoverRef = useRef(null);

	const popoverStylesConfig = useMemo(
		() => getCancelBookingPopoverStyles(theme),
		[theme]
	);

	useOutsideClickMany(
		[cancelPopoverRef, ref as RefObject<HTMLDivElement>],
		handleClose
	);

	return (
		<Popover
			anchorElement={ref && isRefObject(ref) ? ref?.current : null}
			distanceToAnchorElement={distanceToAnchorElement}
			fallbackPlacements={['left', 'top', 'bottom']}
			wrapperStyles={popoverStylesConfig}
			preventOverflow={false}
			withTransition={false}
			ref={cancelPopoverRef}
			preventFlip={false}
			position="right"
			open={isOpen}
		>
			<CancelBookingPopoverWrapper
				onMouseDown={onWrapperClick}
				onClick={onWrapperClick}
			>
				<CancelContentWrapper>
					<Tooltip text={bookedUserInfo} useIfTextCut position="bottom">
						<ClickableRowContentTypography
							variant="m-600"
							onClick={handleInfoClick}
						>
							{bookedUserInfo}
						</ClickableRowContentTypography>
					</Tooltip>

					<ItemRowWithIcon>
						<IconWrapper>
							<CalendarIcon />
						</IconWrapper>
						<Tooltip text={bookedDate} useIfTextCut position="bottom">
							<RowContentTypography variant="m-400">
								{bookedDate}
							</RowContentTypography>
						</Tooltip>
					</ItemRowWithIcon>
					<ItemRowWithIcon>
						<IconWrapper>
							<MapPinIcon />
						</IconWrapper>
						<RowContentTypography variant="m-400">
							{spaceInfo}
						</RowContentTypography>
					</ItemRowWithIcon>

					{bookedDateTime && (
						<ItemRowWithIcon>
							<IconWrapper>
								<ClockIcon />
							</IconWrapper>
							<Tooltip text={bookedDate} useIfTextCut position="bottom">
								<RowContentTypography variant="m-400">
									{bookedDateTime}
								</RowContentTypography>
							</Tooltip>
						</ItemRowWithIcon>
					)}
				</CancelContentWrapper>
				<CancelButton type={type} onClick={handleSubmit} />
			</CancelBookingPopoverWrapper>
		</Popover>
	);
}
