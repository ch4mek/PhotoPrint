import { useTheme } from '@emotion/react';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { spaces } from '@mms/mms-ui-library/theme';
import { ReactNode, useCallback, useMemo, useRef } from 'react';

import { ChevronButton } from '@/components/ChevronButton';
import { discountBreakpoints } from '@/constants/discounts';
import { platePopoverIds } from '@/constants/plateEditor';
import { useToolbarContext } from '@/context/ToolbarContext';

import {
	ButtonsWrapper,
	ChevronButtonWrapper,
	getToolbarPopoverStyles,
	GroupButtonsWrapper,
	GroupTitle,
} from './styles';

const { colorPickerId, toolbarId } = platePopoverIds;

export function GroupToolbarButtonsWrapper({
	children,
	title,
	previewIcon,
}: {
	children: ReactNode;
	title: string;
	previewIcon: ReactNode;
}) {
	const theme = useTheme();
	const isMobileView = !useHasScreenWidth(discountBreakpoints.desktopM);

	const buttonWrapperRef = useRef<HTMLDivElement>(null);
	const popoverRef = useRef<HTMLDivElement>(null);

	const { isToolbarPopoverOpen, handlePopoverClose, handlePopoverOpen } =
		useToolbarContext();
	const { isOpen, type } = isToolbarPopoverOpen;
	const isSelectedType = type === title;

	const popoverStylesConfig = useMemo(
		() => getToolbarPopoverStyles(theme),
		[theme]
	);

	const handleOpen = useCallback(() => {
		handlePopoverOpen(title);
	}, [title]);

	useOutsideClickMany<HTMLButtonElement | HTMLDivElement | null>(
		[buttonWrapperRef, popoverRef],
		handlePopoverClose,
		toolbarId,
		colorPickerId
	);

	return (
		<GroupButtonsWrapper>
			{isMobileView ? (
				<>
					<ChevronButtonWrapper ref={buttonWrapperRef}>
						{previewIcon}
						<ChevronButton
							onClick={handleOpen}
							isActive={isOpen && isSelectedType}
							size="medium"
						/>
					</ChevronButtonWrapper>

					<Popover
						id={platePopoverIds.toolbarId}
						open={isOpen && isSelectedType}
						anchorElement={buttonWrapperRef.current}
						ref={popoverRef}
						distanceToAnchorElement={spaces.s}
						wrapperStyles={popoverStylesConfig}
						withTransition={false}
						position="bottom-start"
					>
						<ButtonsWrapper>{children}</ButtonsWrapper>
					</Popover>
				</>
			) : (
				<ButtonsWrapper>{children}</ButtonsWrapper>
			)}

			{title && <GroupTitle variant="xs-400">{title}</GroupTitle>}
		</GroupButtonsWrapper>
	);
}
