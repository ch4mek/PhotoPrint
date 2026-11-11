import { Header } from '@mms/mms-ui-library/components/Header';
import { LinearLoader } from '@mms/mms-ui-library/components/LinearLoader';
import { LoaderVariant } from '@mms/mms-ui-library/components/LinearLoader/types';
import { Timer } from '@mms/mms-ui-library/components/Timer';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { lazy, Suspense, useRef } from 'react';
import { Outlet, useLocation } from 'react-router-dom';

import { HrChat } from '../components/Chat';
import { useShowUserProfile } from '../components/hooks/useShowUserProfile';
import { LeftMenu } from '../components/LeftMenu';
import { Notifications } from '../components/Notifications';
import { ServiceRequestPopup } from '../components/ServiceRequestPopup';
import { SidebarMenu } from '../components/SidebarMenu';

import { Logo } from '@/components/Logo';
import { getTitleLogoText } from '@/components/Logo/utils';
import { UserMenu } from '@/components/UserMenu';
import {
	SESSION_EXPIRES,
	SESSION_EXPIRES_TEXT,
} from '@/constants/authConstants';
import {
	NotificationType,
	REQUEST_ABSENCE_CREATED,
	REQUEST_DOCUMENTS_CREATED,
} from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { RequestAbsenceForm } from '@/forms/Absence/RequestAbsenceForm';
import { RequestDataValues } from '@/forms/Absence/RequestAbsenceForm/types';
import { convertFormDataToSubmit } from '@/forms/Absence/RequestAbsenceForm/utils/convertFormDataToSubmit';
import { RequestDocumentsForm } from '@/forms/Documents/RequestDocumentsForm';
import { RequestDocumentsValues } from '@/forms/Documents/RequestDocumentsForm/types';
import { convertDocumentsFormDataToSubmit } from '@/forms/Documents/RequestDocumentsForm/utils/convertDocumentsFormDataToSubmit';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createToast } from '@/helpers/createToast';
import { getActiveEventConfig } from '@/helpers/getActiveEventConfig';
import { getHeaderStylesWithBackground } from '@/helpers/getHeaderStylesWithBackground';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useScrollReset } from '@/hooks/useScrollReset';
import { HeaderDecorations } from '@/layouts/components/HeaderParticles';
import {
	AppWrapper,
	LoaderWrapper,
	MainContent,
	CustomScroll,
} from '@/layouts/main/styles';
import { AdditionalContent } from '@/layouts/styles';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateRequest } from '@/queries/inquiries/useCreateRequest';
import { UserClaims } from '@/types/claims';

import {
	bothScrollLayoutRoutes,
	TIMER_VISIBLE_BREAKPOINT,
	HEADER_STYLES,
} from './constants';
import { LayoutProps } from './types';

const UserProfile = lazy(async () => ({
	default: (await import('@/components/UserProfile')).UserProfile,
}));

export function Layout({
	withPaddings = false,
	withScroll = false,
	logoTitle = getTitleLogoText(),
	tabs,
}: LayoutProps) {
	const { id, registeredLocationId, claims } = useAuth();
	const { isOpen, handleOpen, handleClose } = useShowUserProfile();

	const { scrollCallbackRef } = useScrollReset();

	const {
		handleOpen: handleOpenRequest,
		isOpen: isOpenRequest,
		handleClose: handleCloseRequest,
	} = useShowModal();
	const {
		handleOpen: handleOpenPopup,
		isOpen: isPopupOpen,
		handleClose: handleClosePopup,
	} = useShowModal();
	const {
		handleOpen: handleOpenDocumentsRequest,
		isOpen: isOpenDocumentsRequest,
		handleClose: handleCloseDocumentsRequest,
	} = useShowModal();

	const toast = useToast();
	const logoRef = useRef<HTMLHeadingElement>(null);

	const { pathname } = useLocation();

	const isDesktopXSWidth = useHasScreenWidth(breakpoints.desktopXS);
	const isTabletXSWidth = useHasScreenWidth(TIMER_VISIBLE_BREAKPOINT);

	const { mutateAsync: createRequest, isPending: isRequestCreating } =
		useCreateRequest();

	const handleSubmitRequest = (values: RequestDataValues) =>
		createRequest(convertFormDataToSubmit({ ...values, userId: id }), {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, REQUEST_ABSENCE_CREATED)
				);

				handleCloseRequest();
			},
		});

	const handleSubmitDocumentsRequest = (values: RequestDocumentsValues) =>
		createRequest(convertDocumentsFormDataToSubmit({ ...values, userId: id }), {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, REQUEST_DOCUMENTS_CREATED)
				);

				handleCloseDocumentsRequest();
			},
		});

	const displayRequestButton = checkIsHasClaims(
		[UserClaims.ViewAbsences],
		claims
	);

	const activeEventConfig = getActiveEventConfig();
	const headerStyles = getHeaderStylesWithBackground(
		HEADER_STYLES,
		activeEventConfig?.headerBackground
	);

	return (
		<AppWrapper>
			<>
				{activeEventConfig && (
					<HeaderDecorations
						particles={activeEventConfig.particles}
						decorativeIcon={activeEventConfig.decorativeIcon}
						staticDecorations={activeEventConfig.staticDecorations}
					/>
				)}
				<Header
					logo={<Logo title={logoTitle} ref={logoRef} />}
					style={headerStyles}
					navigation={
						<SidebarMenu
							displayModules={!useHasScreenWidth(breakpoints.desktopXS)}
							menuItems={tabs}
						/>
					}
					userMenu={<UserMenu handleOpenUserProfile={handleOpen} />}
					notificationButton={<Notifications />}
					isLeftMenuSidebarOpen={isDesktopXSWidth}
					additionalContent={
						<AdditionalContent>
							{isTabletXSWidth && (
								<Timer
									title={SESSION_EXPIRES_TEXT}
									timestamp={Number(sessionStorage.getItem(SESSION_EXPIRES))}
								/>
							)}
							{displayRequestButton && (
								<ServiceRequestPopup
									isOpen={isPopupOpen}
									onOpen={handleOpenPopup}
									onClose={handleClosePopup}
									onRequestAbsence={() => {
										handleClosePopup();
										handleOpenRequest();
									}}
									onRequestDocuments={() => {
										handleClosePopup();
										handleOpenDocumentsRequest();
									}}
								/>
							)}
						</AdditionalContent>
					}
					isFixed
				/>
				{isDesktopXSWidth && (
					<LeftMenu menuItems={tabs} lastMenuItem={<HrChat />} />
				)}
			</>
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
			{isOpenRequest && (
				<RequestAbsenceForm
					handleClose={handleCloseRequest}
					handleSubmit={handleSubmitRequest}
					initialAbsenceTypeId={undefined}
					locationId={registeredLocationId}
					userId={id}
					{...{ isRequestCreating }}
				/>
			)}
			{isOpenDocumentsRequest && (
				<RequestDocumentsForm
					handleClose={handleCloseDocumentsRequest}
					handleSubmit={handleSubmitDocumentsRequest}
					userId={id}
					{...{ isRequestCreating }}
				/>
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
						ref={scrollCallbackRef}
					>
						<MainContent withPaddings={withPaddings}>
							<Outlet />
						</MainContent>
					</CustomScroll>
				) : (
					<MainContent withPaddings={withPaddings}>
						<Outlet />
					</MainContent>
				)}
			</Suspense>
		</AppWrapper>
	);
}
