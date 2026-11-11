import { useEffect, useMemo } from 'react';
import { useLocation } from 'react-router-dom';

import { getTabIdBySlug, tabsConfig } from '../../main/config';
import { NavigationItemPayload } from '../../main/types';

import { RouteGuard } from '@/components/RouteGuard';
import { Routes } from '@/constants/routes';
import { SIDEBAR_SELECTED_ITEM_STORAGE_KEY } from '@/constants/storageKeys';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/index';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useStickyState } from '@/hooks/useStickyState';

import { SidebarWrapper } from './styles';
import { LeftMenuProps } from './types';

export function LeftMenu({
	isCompactView = false,
	onClose,
	menuItems = tabsConfig,
	lastMenuItem,
}: LeftMenuProps) {
	const location = useLocation();
	const { role, claims: userClaims } = useAuth();

	const userMenuItems = useMemo(
		() =>
			menuItems
				.map((group) =>
					group.filter(({ payload }) => {
						// eslint-disable-next-line spaced-comment
						//TODO: delete the added conditional after all claims for the settings equipment and admin panel and are added
						const { slug } = payload;

						if (
							slug === Routes.settings ||
							slug === Routes.equipment ||
							slug === Routes.adminPanel
						) {
							return (
								checkIsHasClaims(payload.claims, userClaims) ||
								payload?.roles.includes(role)
							);
						}

						if (payload.claims && payload.claims.length) {
							return checkIsHasClaims(payload.claims, userClaims);
						}

						return payload?.roles.includes(role);
					})
				)
				.filter((group) => group.length > 0),
		[menuItems, role, userClaims]
	);

	const flatMenuItems = useMemo(() => userMenuItems.flat(), [userMenuItems]);

	const [selectedMenuItemId, setSelectedMenuItemId] = useStickyState<
		number | null
	>(flatMenuItems[0]?.id || null, SIDEBAR_SELECTED_ITEM_STORAGE_KEY);

	useEffect(() => {
		const currentId = getTabIdBySlug(location.pathname, flatMenuItems);

		setSelectedMenuItemId(currentId);
	}, [location.pathname, flatMenuItems]);

	const onMenuItemClickHandler =
		(setIsShowMenuOpen: (isShowMenuOpen: boolean) => void) => () => {
			onClose?.();
			setIsShowMenuOpen(false);
		};

	return (
		<SidebarWrapper
			isCompactView={isCompactView}
			zIndex={zIndexes.sidebar}
			selectedId={selectedMenuItemId}
			setSelectedId={setSelectedMenuItemId}
			menuItems={userMenuItems}
			lastItem={lastMenuItem}
			render={(
				{ icon, label, id, payload },
				isSelected,
				isHidden,
				setIsShowMoreMenuOpen
			) => {
				const itemPayload = payload as NavigationItemPayload;

				return (
					<RouteGuard
						id={id}
						showTooltip={false}
						key={id}
						icon={icon}
						label={label}
						onClick={onMenuItemClickHandler(setIsShowMoreMenuOpen)}
						selected={isSelected}
						payload={itemPayload}
						isHidden={isHidden}
					/>
				);
			}}
		/>
	);
}
