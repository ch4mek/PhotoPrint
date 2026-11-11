import { Header } from '@mms/mms-ui-library/components/Header';
import { LinearLoader } from '@mms/mms-ui-library/components/LinearLoader';
import { LoaderVariant } from '@mms/mms-ui-library/components/LinearLoader/types';
import { Timer } from '@mms/mms-ui-library/components/Timer';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { lazy, Suspense, useRef } from 'react';
import { Outlet, useLocation } from 'react-router-dom';

import { useShowUserProfile } from '../components/hooks/useShowUserProfile';
import { Notifications } from '../components/Notifications';
import { SidebarMenu } from '../components/SidebarMenu';
import { TIMER_VISIBLE_BREAKPOINT } from '../main/constants';

import { Logo } from '@/components/Logo';
import { APP_FULL_NAME } from '@/components/Logo/constants';
import { UserMenu } from '@/components/UserMenu';
import {
	SESSION_EXPIRES,
	SESSION_EXPIRES_TEXT,
} from '@/constants/authConstants';
import { zIndexes } from '@/constants/zIndexes';
import { getActiveEventConfig } from '@/helpers/getActiveEventConfig';
import { getHeaderStylesWithBackground } from '@/helpers/getHeaderStylesWithBackground';
import { HeaderDecorations } from '@/layouts/components/HeaderParticles';

import { bothScrollLayoutRoutes, HEADER_HEIGHT } from './constants';
import { AppWrapper, LoaderWrapper, CustomScroll, MainContent } from './styles';
import { LayoutProps } from './types';

const UserProfile = lazy(async () => ({
	default: (await import('@/components/UserProfile')).UserProfile,
}));

const HEADER_STYLES = { height: HEADER_HEIGHT, zIndex: zIndexes.header };

export function LayoutWithoutSidebar({
	withScroll = false,
	headerTitle = APP_FULL_NAME,
}: LayoutProps) {
	const { isOpen, handleOpen, handleClose } = useShowUserProfile();
	const isTabletXSWidth = useHasScreenWidth(TIMER_VISIBLE_BREAKPOINT);
	const { pathname } = useLocation();
	const logoRef = useRef<HTMLHeadingElement>(null);

	const activeEventConfig = getActiveEventConfig();
	const headerStyles = getHeaderStylesWithBackground(
		HEADER_STYLES,
		activeEventConfig?.headerBackground
	);

	return (
		<AppWrapper>
			{activeEventConfig && (
				<HeaderDecorations
					particles={activeEventConfig.particles}
					decorativeIcon={activeEventConfig.decorativeIcon}
					staticDecorations={activeEventConfig.staticDecorations}
				/>
			)}
			<Header
				logo={<Logo title={headerTitle} ref={logoRef} />}
				style={headerStyles}
				userMenu={<UserMenu handleOpenUserProfile={handleOpen} />}
				navigation={<SidebarMenu displayModules={false} />}
				notificationButton={<Notifications />}
				additionalContent={
					isTabletXSWidth && (
						<Timer
							title={SESSION_EXPIRES_TEXT}
							timestamp={Number(sessionStorage.getItem(SESSION_EXPIRES))}
						/>
					)
				}
				isFixed
			/>
			{isOpen && (
				<Suspense
					fallback={
						<LoaderWrapper>
							<LinearLoader variant={LoaderVariant.Indeterminate} />
						</LoaderWrapper>
					}
				>
					<UserProfile handleClose={handleClose} />
				</Suspense>
			)}

			<Suspense
				fallback={
					<LoaderWrapper>
						<LinearLoader />
					</LoaderWrapper>
				}
			>
				{withScroll ? (
					<CustomScroll
						maxHeight="100%"
						maxWidth="100%"
						scroll={
							bothScrollLayoutRoutes.includes(pathname)
								? ('both' as 'horizontal')
								: 'horizontal'
						}
					>
						<MainContent>
							<Outlet />
						</MainContent>
					</CustomScroll>
				) : (
					<MainContent>
						<Outlet />
					</MainContent>
				)}
			</Suspense>
		</AppWrapper>
	);
}
