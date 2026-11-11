import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { CandyBoxMenuIcon } from '@mms/mms-ui-library/icons/base/CandyBoxMenuIcon';
import { useRef, useState } from 'react';
import { useLocation } from 'react-router-dom';

import { AppLink } from '../AppLink';
import { HrChat } from '../Chat';
import { GridMenu } from '../GridMenu';
import { LeftMenu } from '../LeftMenu';

import { IconSize } from '@/constants/iconSizes';
import { useResize } from '@/hooks/useResize';
import { HEADER_ACTION_BUTTON_STYLES } from '@/layouts/main/constants';

import {
	appModsenLink,
	headerMenuLinks,
	SIDEBAR_POPOVER_OFFSET,
	sidebarMenuSections,
} from './constants';
import { OtherAppWrapper, SidebarMenuWrapper } from './styles';
import { SidebarMenuProps } from './types';
import { checkIfCurrentHrefNotModsenApp, getModulesAppLinks } from './utils';

const { other, modules, chatWidget } = sidebarMenuSections;

export function SidebarMenu({ displayModules, menuItems }: SidebarMenuProps) {
	const buttonRef = useRef(null);
	const popoverRef = useRef(null);

	const [, windowHeight] = useResize();
	const { pathname } = useLocation();

	const appLinks = getModulesAppLinks();

	const [open, setOpen] = useState(false);

	const isModsenApp = headerMenuLinks.every(
		checkIfCurrentHrefNotModsenApp(pathname)
	);

	const handleDrawerOpen = () => {
		setOpen((prev) => !prev);
	};

	const handleCloseMenu = () => {
		setOpen(false);
	};

	useOutsideClickMany([buttonRef, popoverRef], handleCloseMenu);

	return (
		<>
			<ActionButton
				ref={buttonRef}
				aria-label="open drawer"
				isActive={open}
				onClick={handleDrawerOpen}
				color="white"
				size="small"
				style={HEADER_ACTION_BUTTON_STYLES}
			>
				<CandyBoxMenuIcon width={IconSize.M} height={IconSize.M} />
			</ActionButton>
			<Popover
				ref={popoverRef}
				open={open}
				anchorElement={buttonRef?.current}
				position={displayModules ? 'bottom' : 'bottom-start'}
				offset={SIDEBAR_POPOVER_OFFSET}
				fallbackPlacements={['bottom']}
			>
				<SidebarMenuWrapper
					displayModules={displayModules}
					windowHeight={windowHeight}
					isMobileView={displayModules}
				>
					<OtherAppWrapper>
						{!isModsenApp && (
							<GridMenu>
								<AppLink onClick={handleCloseMenu} {...appModsenLink} />
							</GridMenu>
						)}
						<GridMenu title={displayModules ? other : undefined}>
							{appLinks.map((linkInfo, index) => (
								<AppLink onClick={handleCloseMenu} key={index} {...linkInfo} />
							))}
						</GridMenu>
					</OtherAppWrapper>
					{displayModules && (
						<GridMenu title={displayModules ? modules : undefined}>
							<LeftMenu
								isCompactView
								onClose={handleCloseMenu}
								menuItems={menuItems}
							/>
						</GridMenu>
					)}
					{displayModules && (
						<GridMenu title={displayModules ? chatWidget : undefined}>
							<HrChat />
						</GridMenu>
					)}
				</SidebarMenuWrapper>
			</Popover>
		</>
	);
}
