import { useTheme } from '@emotion/react';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { breakpoints, spaces } from '@mms/mms-ui-library/theme';
import { useMemo, useRef } from 'react';

import { Chat } from '@/components/ChatWidget/components/Chat';
import { ChatWidgetProps } from '@/components/ChatWidget/components/types';
import {
	BORDER_WIDTH,
	DESKTOP_TRANSFORM,
} from '@/components/ChatWidget/constants';
import { getChatWidgetPopoverStyles } from '@/components/ChatWidget/styles';
import { isRefObject } from '@/helpers/isRefObject';
import { useResize } from '@/hooks/useResize';
import { CHAT_WIDGET_ANCHOR_ELEMENT_ID } from '@/layouts/components/ChatWidgetButton/constants';
import { HEADER_HEIGHT } from '@/layouts/main/constants';

const { desktopXS } = breakpoints;

export function ChatWidget({ isOpen, handleClose, ref }: ChatWidgetProps) {
	const theme = useTheme();
	const popoverRef = useRef<HTMLDivElement>(null);
	const isDesktopXSWidth = useHasScreenWidth(desktopXS);
	const [, windowHeight] = useResize();

	const popoverStylesConfig = useMemo(
		() => getChatWidgetPopoverStyles(theme, isDesktopXSWidth),
		[theme, isDesktopXSWidth]
	);

	useOutsideClickMany(
		[popoverRef],
		handleClose,
		undefined,
		CHAT_WIDGET_ANCHOR_ELEMENT_ID
	);

	const popoverProps = useMemo(() => {
		if (isDesktopXSWidth) {
			return {
				position: 'right-bottom',
				distanceToAnchorElement: spaces.s,
				popperStyles: {
					transform: DESKTOP_TRANSFORM,
				},
			};
		}

		return {
			position: 'bottom',
			distanceToAnchorElement: 0,
			popperStyles: {
				width: '100%',
				transform: `translate(0px, ${
					HEADER_HEIGHT - windowHeight + BORDER_WIDTH
				}px)`,
			},
		};
	}, [isDesktopXSWidth, windowHeight]);

	return (
		<Popover
			open={isOpen}
			anchorElement={ref && isRefObject(ref) ? ref?.current : null}
			ref={popoverRef}
			wrapperStyles={popoverStylesConfig}
			{...{ ...popoverProps }}
		>
			<Chat handleClose={handleClose} />
		</Popover>
	);
}
