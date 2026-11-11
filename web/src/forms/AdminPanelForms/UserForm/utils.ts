import { Theme } from '@emotion/react';

import {
	BORDER_HEIGHT,
	MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT,
	POPUP_BUTTONS_CONTAINER_HEIGHT,
	POPUP_HEADER_HEIGHT,
	POPUP_TABS_HEIGHT,
	VERTICAL_PADDINGS,
	popUpBreakpoints,
	popUpPaddings,
	popUpSizes,
} from '@/constants/popUp';
import { isGroupManager } from '@/constants/roles';
import { DEFAULT_USER_COLOR } from '@/constants/user';
import { UserFormValues } from '@/forms/types';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import {
	DepartmentType,
	LocationType,
	RoleType,
	CalendaryCountry,
} from '@/types/configuration';
import { DepartmentGroup, DepartmentResponse } from '@/types/department';
import { NullableRole } from '@/types/roles';
import { UseUserResponse } from '@/types/userInfo';

import {
	USER_FORM_POPUP_HEIGHT_TABLET,
	USER_FORM_TABS_HEADER,
	UserFormTabs,
} from './constants';
import { GetPopUpStylesConfigProps, GetTabWrapperHeightProps } from './types';

export const getUserCurrentValues = (
	userData: UseUserResponse | undefined,
	initialValues: UserFormValues,
	departmentTypes: Array<DepartmentType>,
	roleTypes: Array<RoleType>,
	locationTypes: Array<LocationType>,
	registeredLocationTypes: Array<CalendaryCountry>,
	departmentsInfo: Array<DepartmentResponse> | undefined
): UserFormValues =>
	userData
		? {
				...userData,
				firstNameRu: userData.firstNameRu ?? '',
				lastNameRu: userData.lastNameRu ?? '',
				patronymicRu: userData.patronymicRu ?? '',
				color: userData.color || DEFAULT_USER_COLOR,
				department:
					departmentTypes.find(({ id }) => id === userData?.departmentId) ??
					null,
				position:
					roleTypes.find(({ name }) => name === userData?.position) ?? null,
				location:
					locationTypes.find(({ name }) => name === userData?.location) ?? null,
				projects: [],
				currentLocation: userData.currentLocation ?? '',
				registeredLocation:
					registeredLocationTypes.find(
						({ name }) => name === userData?.registeredLocation
					) ?? null,
				registeredLocationId:
					registeredLocationTypes.find(
						({ id }) => id === userData.registeredLocationId
					)?.id ?? null,
				group:
					departmentsInfo
						?.find(
							({ departmentName }) =>
								departmentName ===
								departmentTypes.find(({ id }) => userData.departmentId === id)
									?.fullName
						)
						?.groups?.find(({ id }) => id === userData.resourceGroupId) ?? null,
		  }
		: initialValues;

export const getUserFormTabs = (displayGroupMembers: boolean) =>
	USER_FORM_TABS_HEADER.filter(({ id }) => {
		if (!displayGroupMembers && id === UserFormTabs.GROUP_MEMBERS) {
			return false;
		}

		return true;
	});

const getControlsContainerHeight = (windowWidth: number) =>
	windowWidth < popUpBreakpoints.tabletXS
		? MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT
		: POPUP_BUTTONS_CONTAINER_HEIGHT;

const getAdaptivePopupMaxHeight = (windowHeight: number) =>
	windowHeight - VERTICAL_PADDINGS;

const getContentVerticalPadding = (activeTab: UserFormTabs, theme: Theme) => {
	const projectTabs = [UserFormTabs.PROJECT_ACCESS, UserFormTabs.GROUP_MEMBERS];

	if (projectTabs.includes(activeTab)) {
		return theme.spaces.s + theme.spaces.l;
	}

	return theme.spaces.l + theme.spaces.m;
};

const getContentMaxHeightPx = (
	popupHeight: number,
	windowHeight: number,
	hasControls: boolean
) => {
	const controlsHeight = hasControls ? POPUP_BUTTONS_CONTAINER_HEIGHT : 0;

	const viewportMaxHeight =
		windowHeight -
		POPUP_TABS_HEIGHT -
		controlsHeight -
		VERTICAL_PADDINGS -
		POPUP_HEADER_HEIGHT -
		BORDER_HEIGHT;

	const containerMaxHeight =
		popupHeight -
		POPUP_HEADER_HEIGHT -
		POPUP_TABS_HEIGHT -
		BORDER_HEIGHT -
		controlsHeight;

	return Math.min(viewportMaxHeight, containerMaxHeight);
};

const getUserFormPopupHeight = (windowHeight: number, windowWidth: number) => {
	const adaptivePopupMaxHeight = getAdaptivePopupMaxHeight(windowHeight);

	if (windowWidth >= popUpBreakpoints.tablet) {
		return adaptivePopupMaxHeight;
	}

	return Math.min(USER_FORM_POPUP_HEIGHT_TABLET, adaptivePopupMaxHeight);
};

export const getTabWrapperHeight = ({
	theme,
	activeTab,
	windowHeight,
	windowWidth,
	userTabHeight,
}: GetTabWrapperHeightProps) => {
	if (windowWidth >= popUpBreakpoints.tablet) {
		return (
			userTabHeight + getControlsContainerHeight(windowWidth) + theme.spaces.s
		);
	}

	const hasControls = activeTab === UserFormTabs.USER;
	const popupHeight = getUserFormPopupHeight(windowHeight, windowWidth);
	const contentMaxHeight = getContentMaxHeightPx(
		popupHeight,
		windowHeight,
		hasControls
	);

	return contentMaxHeight - getContentVerticalPadding(activeTab, theme);
};

const getPopUpContentPadding = (activeTab: UserFormTabs) => {
	if (activeTab === UserFormTabs.USER) {
		return popUpPaddings.hasControls;
	}

	const projectTabs = [UserFormTabs.PROJECT_ACCESS, UserFormTabs.GROUP_MEMBERS];

	if (projectTabs.includes(activeTab)) {
		return popUpPaddings.projectsTab;
	}

	return popUpPaddings.default;
};

export const getPopUpStylesConfig = ({
	theme,
	hasTabs,
	activeTab,
	windowHeight,
	windowWidth,
}: GetPopUpStylesConfigProps) => {
	const hasControls = activeTab === UserFormTabs.USER;
	const isTabletWidth = windowWidth < popUpBreakpoints.tablet;
	const popupHeight = getUserFormPopupHeight(windowHeight, windowWidth);
	const contentMaxHeight = getContentMaxHeightPx(
		popupHeight,
		windowHeight,
		hasControls
	);

	return {
		container: {
			width: popUpSizes.m,

			[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
				width: popUpSizes.s,
			},

			[theme.breakpoints.down(popUpBreakpoints.tablet)]: {
				...(isTabletWidth && {
					height: popupHeight,
					maxHeight: getAdaptivePopupMaxHeight(windowHeight),
				}),
			},
		},
		content: {
			...getAdaptivePopUpContentStylesConfig({
				theme,
				hasTabs,
				hasContols: hasControls,
				windowHeight,
				windowWidth,
			}),
			maxHeight: `${contentMaxHeight}px`,

			padding: getPopUpContentPadding(activeTab),
		},
		button: {
			flexWrap: 'wrap' as const,
		},
	};
};

export const getAvailableGroupsForUser = (
	groups: Array<DepartmentGroup>,
	role?: NullableRole,
	currentGroup?: DepartmentGroup
): Array<DepartmentGroup> =>
	groups.filter(
		({ groupManagerEmail, id }) =>
			!(isGroupManager(role) && groupManagerEmail && currentGroup?.id !== id)
	);
