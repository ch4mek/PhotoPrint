import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import useMediaQuery from '@mui/material/useMediaQuery';
import { SetStateAction, useCallback, useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { PersonalInfo } from '@/components/PersonalInfo';
import { TabPanel } from '@/components/TabPanel';
import { UpcomingReview } from '@/components/UpcomingReview';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useAvatar } from '@/hooks/useAvatar';
import { useResize } from '@/hooks/useResize';
import { useTabs } from '@/hooks/useTabs';

import { TeamsChatButton } from './components/TeamBoardUserSummaryTab/TeamsChatButton';
import { UserAvatar } from './components/UserAvatar';
import { POPUP_TITLE, tabs } from './constants';
import { PersonalDataContext } from './context/personalDataContext';
import { TabContext } from './context/tabContext';
import { useTabsSectionHeight } from './hooks/useTabsSectionHeight';
import { useTeamboardUser } from './hooks/useTeamboardUser';
import { ContentForm, ContentHeader, TabsSection } from './styles';
import { TeamBoardUserTabs } from './TeamBoardUserTabs';
import {
	PersonalDataContextValues,
	TeamBoardTabsId,
	TeamboardUserPageProps,
} from './types';
import {
	filterTabsByProfileKind,
	getPopUpStylesConfig,
	hasAccess,
	isEmployeeProfile,
	isFullAccess,
	isInternProfile,
} from './utils';
import { getSummaryTabComponent } from './utils/getSummaryTabComponent';

export function TeamboardUserPage({
	id,
	handleClose,
	setPersonalDataChanged,
}: TeamboardUserPageProps) {
	const theme = useTheme();
	const [preferredTab, setPreferredTab] = useTabs(TeamBoardTabsId.SUMMARY);
	const [, windowHeight] = useResize();
	const isDownTabletXS = useMediaQuery(
		theme.breakpoints.down(theme.breakpoints.tabletXS)
	);

	const { claims } = useAuth();

	const handleTabChange = useCallback((newTab: SetStateAction<number>) => {
		setPreferredTab(newTab);
	}, []);

	const { data, isFetching, isLoading, accessLevel, profileKind, refetch } =
		useTeamboardUser(id, handleClose, setPersonalDataChanged);

	const isDataLoading = isFetching || isLoading;

	const personalDataContextValue = useMemo(
		() =>
			({
				data: data || {},
				isFetching,
				isLoading,
				showLoader: !data,
				refetch,
				setPersonalDataChanged,
				userId: id,
				position: data?.position,
				accessLevel,
				profileKind,
			} as PersonalDataContextValues),
		[data, isFetching, refetch, isLoading, id, accessLevel, profileKind]
	);

	const {
		upcomingPerfomanceReview,
		performanceReviewDateStatus,
		primarySpecialization,
		firstName,
		lastName,
		fullName,
		seniority,
		comments,
		avatarFileId,
		needProject,
		departmentId,
		email,
	} = personalDataContextValue.data;

	const { imgSource } = useAvatar(avatarFileId || '');

	const tabContextValue = useMemo(
		() => ({
			handleTabChange,
		}),
		[handleTabChange]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const tabsToShow = useMemo(
		() =>
			filterTabsByProfileKind(
				tabs.filter(
					(tab) =>
						hasAccess(accessLevel, tab.accessLevels) &&
						(!tab.requiredClaims ||
							checkIsHasClaims(tab.requiredClaims, claims))
				),
				profileKind
			).map((tab) =>
				tab.id === TeamBoardTabsId.SUMMARY
					? { ...tab, Component: getSummaryTabComponent(profileKind) }
					: tab
			),
		[accessLevel, claims, profileKind]
	);

	const activeTab = useMemo(() => {
		if (!tabsToShow.length) {
			return TeamBoardTabsId.SUMMARY;
		}

		return tabsToShow.some((tab) => tab.id === preferredTab)
			? preferredTab
			: tabsToShow[0].id;
	}, [tabsToShow, preferredTab]);

	const { minHeight, maxHeight } = useTabsSectionHeight({
		activeTab,
		position: data?.position,
		departmentId,
		accessLevel,
	});

	const isFullPopup = isFullAccess(accessLevel);
	const isEmployee = isEmployeeProfile(profileKind);
	const isIntern = isInternProfile(profileKind);

	const showUpcomingReview = isEmployee && isFullPopup;
	const showTeamsChatInHeader =
		isEmployee && (isFullPopup || (!isFullPopup && isDownTabletXS));
	const showTeamsChatInPersonalInfo =
		isEmployee && !isFullPopup && !isDownTabletXS;
	const showContentHeader = showUpcomingReview || showTeamsChatInHeader;

	return (
		<PopUp
			zIndex={zIndexes.profilePopup}
			title={POPUP_TITLE}
			onClose={handleClose}
			stylesConfig={popUpStylesConfig}
			type="info"
			hasPaddings={false}
		>
			<TabContext.Provider value={tabContextValue}>
				<PersonalDataContext.Provider value={personalDataContextValue}>
					<Loader isLoading={isDataLoading} size="small">
						<ContentForm>
							{showContentHeader && (
								<ContentHeader hasUpcomingReview={showUpcomingReview}>
									{showUpcomingReview && (
										<UpcomingReview
											{...{
												isFetching,
												isLoading,
												upcomingPerfomanceReview,
												performanceReviewDateStatus,
											}}
										/>
									)}
									{showTeamsChatInHeader && <TeamsChatButton {...{ email }} />}
								</ContentHeader>
							)}
							<PersonalInfo
								{...{
									needProject,
									seniority,
									primarySpecialization,
									fullName,
									comments,
									refetch,
								}}
								position={isIntern ? data?.position : undefined}
								withNeedAProjectButton={isEmployee && isFullPopup}
								isShowLoader={isFetching || isLoading}
								isShowSpecialization={isEmployee && isFullPopup}
								isShowComments={isFullPopup}
								AvatarComponent={UserAvatar}
								TeamsChatButtonComponent={
									showTeamsChatInPersonalInfo ? (
										<TeamsChatButton {...{ email }} />
									) : undefined
								}
								userId={id}
								avatarProps={{
									src: imgSource || '',
									firstName,
									lastName,
									avatarFileId,
								}}
							/>
							<TabsSection {...{ minHeight, maxHeight }}>
								<TeamBoardUserTabs
									{...{ activeTab, accessLevel, profileKind }}
									onChange={handleTabChange}
								>
									{tabsToShow.map(({ id: tabId, Component }) => (
										<TabPanel key={tabId} index={tabId} value={activeTab}>
											<Component />
										</TabPanel>
									))}
								</TeamBoardUserTabs>
							</TabsSection>
						</ContentForm>
					</Loader>
				</PersonalDataContext.Provider>
			</TabContext.Provider>
		</PopUp>
	);
}
