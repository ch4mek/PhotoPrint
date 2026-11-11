import { lazy, useEffect } from 'react';
import { Routes as BrowserRoutes, Navigate, Route } from 'react-router-dom';

import { DynamicMFERoute } from '@/components/DynamicMFERoute';
import {
	APP_MODSEN_DISCOUNTS_NAME,
	APP_MODSEN_WIKI_NAME,
} from '@/components/Logo/constants';
import { PrivateRoute } from '@/components/PrivateRoute';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import {
	allRolesExceptIntern,
	chiefRoles,
	managerRolesWithGroupManager,
	managerRolesWithGroupManagerAndSA,
	Roles,
} from '@/constants/roles';
import { DEFAULT_NAVIGATION_ROUTE, Routes } from '@/constants/routes';
import { useToast } from '@/context/index';
import { useBodyBackgroundColor } from '@/hooks/useBodyBackground';
import { Layout } from '@/layouts/main';
import { LayoutWithoutSidebar } from '@/layouts/mainWithoutSidebar';
import { RedirectPage } from '@/pages/RedirectPage';
import { SignInPage } from '@/pages/SignIn';
import { UserClaims } from '@/types/claims';

const SetProfilePage = lazy(async () => ({
	default: (await import('@/pages/SetProfile')).SetProfile,
}));
const TimeSheetPage = lazy(async () => ({
	default: (await import('@/pages/TimeSheet')).TimeSheetPage,
}));
const ReportsPage = lazy(async () => ({
	default: (await import('@/pages/Reports')).ReportsPage,
}));
const HomePage = lazy(async () => ({
	default: (await import('@/pages/Home')).HomePage,
}));
const DashboardPage = lazy(async () => ({
	default: (await import('@/pages/Dashboard')).DashboardPage,
}));
const BookingPage = lazy(async () => ({
	default: (await import('@/pages/Booking')).BookingPage,
}));
const AuthCallbackPage = lazy(async () => ({
	default: (await import('@/pages/AuthCallback')).AuthCallbackPage,
}));
const EquipmentPage = lazy(async () => ({
	default: (await import('@/pages/Equipment')).EquipmentPage,
}));

const MerchPage = lazy(async () => ({
	default: (await import('@/pages/Merch')).MerchPage,
}));

const DiscountsPage = lazy(async () => ({
	default: (await import('@/pages/Discounts')).DiscountsPage,
}));

const AdminPanelPage = lazy(async () => ({
	default: (await import('@/pages/AdminPanel')).AdminPanelPage,
}));
const SettingsPage = lazy(async () => ({
	default: (await import('@/pages/Settings')).SettingsPage,
}));

const UserProfile = lazy(async () => ({
	default: (await import('@/components/UserProfile')).UserProfile,
}));

const Notifications = lazy(async () => ({
	default: (await import('@/layouts/components/Notifications')).Notifications,
}));

const HrChat = lazy(async () => ({
	default: (await import('@/layouts/components/Chat')).HrChat,
}));

const TeamboardPage = lazy(async () => ({
	default: (await import('@/pages/Teamboard')).TeamboardPage,
}));
const PerformanceReviewPage = lazy(async () => ({
	default: (await import('@/pages/PerformanceReview')).PerformanceReviewPage,
}));
const PayloadPage = lazy(async () => ({
	default: (await import('@/pages/Payload')).PayloadPage,
}));
const WikiPage = lazy(async () => ({
	default: (await import('@/pages/Wiki')).WikiPage,
}));

const WelcomeWikiPage = lazy(async () => ({
	default: (await import('@/pages/Wiki/components/WelcomeWikiLayout'))
		.WelcomeWikiLayout,
}));

const ArticlePage = lazy(async () => ({
	default: (await import('@/pages/Wiki/components/ArticlePage')).ArticlePage,
}));

const VacationsPage = lazy(async () => ({
	default: (await import('@/pages/Vacations')).VacationsPage,
}));
const RequestsPage = lazy(async () => ({
	default: (await import('@/pages/Requests')).RequestsPage,
}));

const MicrofrontendsPanelPage = lazy(async () => ({
	default: (await import('@/pages/MicrofrontendsPanel'))
		.MicrofrontendsPanelPage,
}));

const AttendacePage = lazy(async () => ({
	default: (await import('@/pages/Attendance')).AttendancePage,
}));

const StaffingPage = lazy(async () => ({
	default: (await import('@/pages/Staffing')).StaffingPage,
}));

const LabboardPage = lazy(async () => ({
	default: (await import('@/pages/Labboard')).LabboardPage,
}));

const ProfitAndLossPage = lazy(async () => ({
	default: (await import('@/pages/ProfitAndLoss')).ProfitAndLossPage,
}));

export function AppRouter() {
	useBodyBackgroundColor();
	const toast = useToast();

	useEffect(() => {
		toast.subscribe();
	}, []);

	return (
		<BrowserRoutes>
			<Route path={Routes.signIn} element={<SignInPage />} />
			<Route path={Routes.redirectToExternalLink} element={<RedirectPage />} />
			<Route path={Routes.authCallback} element={<AuthCallbackPage />} />
			<Route path={Routes.setProfile} element={<SetProfilePage />} />
			<Route element={<ProtectedRoute />}>
				<Route
					element={
						<LayoutWithoutSidebar
							withScroll
							headerTitle={APP_MODSEN_WIKI_NAME}
						/>
					}
				>
					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ViewWiki]} />
						}
					>
						<Route path={Routes.wiki} element={<WikiPage />}>
							<Route index element={<WelcomeWikiPage />} />
							<Route path={Routes.wikiArticleId} element={<ArticlePage />} />
						</Route>
					</Route>
				</Route>
				<Route
					element={
						<PrivateRoute
							available={[Roles.CTO]}
							availablePermissions={[UserClaims.AccessToRecruitment]}
						/>
					}
				>
					<Route
						path={Routes.recruitment}
						element={
							<DynamicMFERoute {...{ UserProfile, Notifications, HrChat }} />
						}
					/>
				</Route>
				<Route
					element={
						<LayoutWithoutSidebar
							withScroll
							headerTitle={APP_MODSEN_DISCOUNTS_NAME}
						/>
					}
				>
					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ViewDiscounts]} />
						}
					>
						<Route path={Routes.discounts} element={<DiscountsPage />} />
						<Route path={Routes.discountId} element={<DiscountsPage />} />
					</Route>
				</Route>
				<Route element={<Layout withPaddings withScroll />}>
					<Route index element={<Navigate to={DEFAULT_NAVIGATION_ROUTE} />} />
					<Route element={<PrivateRoute available={allRolesExceptIntern} />}>
						<Route path={Routes.reports} element={<ReportsPage />} />
					</Route>
					<Route path={Routes.timeSheet} element={<TimeSheetPage />} />

					<Route element={<PrivateRoute available={chiefRoles} />}>
						<Route path={Routes.vacations} element={<VacationsPage />} />
					</Route>

					<Route
						element={<PrivateRoute available={managerRolesWithGroupManager} />}
					>
						<Route
							path={Routes.performanceReview}
							element={<PerformanceReviewPage />}
						/>
						<Route path={Routes.payload} element={<PayloadPage />} />
						<Route path={Routes.teamBoard} element={<TeamboardPage />} />
					</Route>

					<Route
						element={
							<PrivateRoute
								availablePermissions={[
									UserClaims.ManageInterns,
									UserClaims.ManageAllInterns,
								]}
							/>
						}
					>
						<Route path={Routes.labboard} element={<LabboardPage />} />
					</Route>

					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ManageMfe]} />
						}
					>
						<Route
							path={Routes.microfrontendsPanel}
							element={<MicrofrontendsPanelPage />}
						/>
					</Route>

					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ViewHome]} />
						}
					>
						<Route path={Routes.home} element={<HomePage />} />
					</Route>

					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ViewDashboard]} />
						}
					>
						<Route path={Routes.dashboard} element={<DashboardPage />} />
					</Route>
				</Route>

				<Route element={<Layout />}>
					<Route index element={<Navigate to={DEFAULT_NAVIGATION_ROUTE} />} />
					<Route element={<PrivateRoute available={allRolesExceptIntern} />}>
						<Route path={Routes.booking} element={<BookingPage />} />
					</Route>

					<Route
						element={
							<PrivateRoute
								available={managerRolesWithGroupManager}
								availablePermissions={[UserClaims.ManageAbsenceSettings]}
							/>
						}
					>
						<Route path={Routes.settings} element={<SettingsPage />} />
					</Route>
					<Route
						element={
							<PrivateRoute
								available={managerRolesWithGroupManagerAndSA}
								availablePermissions={[UserClaims.ManageAllEquipment]}
							/>
						}
					>
						<Route path={Routes.equipment} element={<EquipmentPage />} />
					</Route>
					<Route
						element={
							<PrivateRoute
								available={managerRolesWithGroupManagerAndSA}
								availablePermissions={[
									UserClaims.ManageHRModule,
									UserClaims.ManageProjectCoordinator,
									UserClaims.ManageAccountant,
								]}
							/>
						}
					>
						<Route path={Routes.adminPanel} element={<AdminPanelPage />} />
					</Route>
					<Route
						element={
							<PrivateRoute availablePermissions={[UserClaims.ViewMerch]} />
						}
					>
						<Route path={Routes.merch} element={<MerchPage />} />
					</Route>
					<Route
						element={<PrivateRoute available={managerRolesWithGroupManager} />}
					>
						<Route path={Routes.attendance} element={<AttendacePage />} />
					</Route>
					<Route
						element={
							<PrivateRoute
								availablePermissions={[
									UserClaims.ManageAllRequests,
									UserClaims.ManageRequests,
									UserClaims.ViewAllRequests,
								]}
							/>
						}
					>
						<Route path={Routes.requests} element={<RequestsPage />} />
					</Route>
					<Route
						element={
							<PrivateRoute
								availablePermissions={[
									UserClaims.ManageAllInterviews,
									UserClaims.ManageInterviews,
									UserClaims.ViewProjectExits,
									UserClaims.ManageAllProjectExits,
								]}
							/>
						}
					>
						<Route path={Routes.staffing} element={<StaffingPage />} />
					</Route>
					<Route
						element={
							<PrivateRoute
								availablePermissions={[
									UserClaims.ViewRates,
									UserClaims.ViewAllRates,
									UserClaims.ViewManagementTab,
									UserClaims.ManageExpensesTab,
								]}
							/>
						}
					>
						<Route
							path={Routes.profitAndLoss}
							element={<ProfitAndLossPage />}
						/>
					</Route>
				</Route>

				{/* Роуты для микрофронтендов */}
				<Route element={<Layout withScroll />}>
					<Route path={Routes.all} element={<DynamicMFERoute />} />
				</Route>
			</Route>
		</BrowserRoutes>
	);
}
