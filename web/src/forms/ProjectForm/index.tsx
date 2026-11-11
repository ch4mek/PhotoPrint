import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { SingleTab } from '@mms/mms-ui-library/components/Tabs';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { FormikHelpers } from 'formik';
import { useCallback, useLayoutEffect, useMemo, useState } from 'react';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { PopUpTitle } from '@/components/PopUpTitle';
import { TabPanel } from '@/components/TabPanel';
import {
	ACTIVATE,
	DEACTIVATE,
	getAddingPopUpTitle,
	getEditingDeactivatedEntityMessage,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	isChiefRole,
	isGroupManager,
	isHeadOfDepartment,
} from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import type {
	ProjectFormProps,
	ProjectFormValues,
} from '@/forms/ProjectForm/types';
import { removeExtraSpaces } from '@/helpers/removeExtraSpaces';
import { useResize } from '@/hooks/useResize';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useProject } from '@/queries/useProject';
import { ClientsLookupResponse } from '@/types/clients';

import {
	CAPITALIZED_PROJECT_FORM_NAME,
	PROJECT_FORM_NAME,
	PROJECT_FORM_TABS,
	ProjectFromTabId,
} from './constants';
import { ProjectTab } from './ProjectTab';
import { validationSchema } from './schema';
import { ClientName, ProjectInfo, ProjectName, TabWrapper } from './styles';
import { UserAccessTab } from './UserAccessTab';
import { getPopUpStylesConfig, getTabWrapperHeight } from './utils';

export function ProjectForm({
	initialValues,
	handleClose,
	onSubmit,
	id,
	onChangeStatus,
}: ProjectFormProps) {
	const theme = useTheme();
	const [windowWidth, windowHeight] = useResize();
	const { role } = useAuth();

	const {
		data: projectData,
		isLoading: isLoadingProject,
		isFetching: isFetchingProject,
	} = useProject(id);

	const { data: clientOptions } = useClientsLookup({
		isDeleted: false,
		isActive: true,
	});

	const {
		isOpen: isActivityFormOpen,
		handleClose: handleActivityFormClose,
		handleOpen: handleActivityFormOpen,
	} = useShowModal();

	const [isFormDisabled, setIsFormDisabled] = useState(false);
	const [activeTab, setActiveTab] = useState(ProjectFromTabId.PROJECT);

	useLayoutEffect(() => {
		if (projectData) {
			setIsFormDisabled(() => !projectData.isActive || isGroupManager(role));
		}
	}, [projectData]);

	const handleChangeTab = useCallback((newActiveTab: number) => {
		setActiveTab(newActiveTab);
	}, []);

	const projectFormTabs = useMemo(() => {
		const tabs: SingleTab[] = [PROJECT_FORM_TABS.project];

		if (id) {
			tabs.push({
				disabled: projectData ? projectData.isPublic : true,
				...PROJECT_FORM_TABS.userAccess,
			});
		}

		return {
			data: tabs,
			onChangeTab: handleChangeTab,
			activeTab,
		};
	}, [id, activeTab, projectData?.isPublic]);

	const handleSubmit = useCallback(
		async (
			values: ProjectFormValues,
			{ setSubmitting }: FormikHelpers<ProjectFormValues>
		) => {
			const projectsValues = values.isPublic
				? { ...values, users: [] }
				: values;

			const castValues = removeExtraSpaces(projectsValues, validationSchema);

			const isSubmited = await onSubmit(
				castValues as unknown as ProjectFormValues
			);
			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[]
	);

	const projectClient = clientOptions?.find(
		(el) => el.id === projectData?.clientId
	) as unknown as ClientsLookupResponse[] & {
		id: number;
		name: string;
	};

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, activeTab, windowHeight, windowWidth),
		[theme, activeTab, windowHeight, windowWidth]
	);

	const isLoading = Boolean(isLoadingProject && id);

	const isActivateButtonVisible =
		(isHeadOfDepartment(role) && projectData?.isSelfCreatedProject) ||
		isChiefRole(role);

	const tabWrapperHeight = useMemo(
		() =>
			`${getTabWrapperHeight({
				theme,
				activeTab,
				windowHeight,
				windowWidth,
			})}px`,
		[theme, activeTab, windowHeight, windowWidth]
	);

	return (
		<FormikWithBackup
			validateOnBlur={false}
			enableReinitialize
			initialValues={
				projectData
					? {
							id: projectData.id,
							name: projectData.name,
							client: projectClient,
							isPublic: projectData.isPublic,
							isBillable: projectData.isBillable,
							color: projectData.color,
							isActive: projectData.isActive,
							manager:
								projectData.managerId && projectData.managerFullName
									? {
											id: projectData.managerId,
											fullName: projectData.managerFullName,
									  }
									: null,
					  }
					: initialValues
			}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validationSchema={validationSchema}
			name={PROJECT_FORM_NAME}
			id={id}
		>
			{({ isSubmitting, values, submitForm, handleOutsideClose }) => (
				<PopUp
					title={
						id ? (
							<PopUpTitle
								headerText={getEditingPopUpTitle(CAPITALIZED_PROJECT_FORM_NAME)}
								extraContent={
									projectData &&
									projectClient && (
										<ProjectInfo variant="m-400">
											(
											<Tooltip
												text={projectData.name}
												useIfTextCut
												fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
											>
												<ProjectName>{projectData.name}</ProjectName>
											</Tooltip>
											<Tooltip
												text={projectClient.name}
												useIfTextCut
												fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
											>
												<ClientName>{`/ ${projectClient.name}`}</ClientName>
											</Tooltip>
											)
										</ProjectInfo>
									)
								}
							/>
						) : (
							getAddingPopUpTitle(CAPITALIZED_PROJECT_FORM_NAME)
						)
					}
					tabs={projectFormTabs}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					stylesConfig={popUpStylesConfig}
					type="form"
					controls={
						activeTab === PROJECT_FORM_TABS.project.id
							? {
									extraControls:
										id && isActivateButtonVisible
											? [
													{
														text: values.isActive ? DEACTIVATE : ACTIVATE,
														view: 'light',
														color: values.isActive ? 'negative' : 'primary',
														onClick: handleActivityFormOpen,
													},
											  ]
											: undefined,
									positiveControl: {
										onClick: submitForm,
										disabled: isFormDisabled,
										tooltipText: isFormDisabled
											? getEditingDeactivatedEntityMessage(PROJECT_FORM_NAME)
											: undefined,
									},
									negativeControl: { onClick: handleClose },
							  }
							: undefined
					}
					showLoader={isLoading || isSubmitting || isFetchingProject}
				>
					<TabPanel value={activeTab} index={ProjectFromTabId.PROJECT}>
						<TabWrapper height={tabWrapperHeight}>
							<ProjectTab
								id={id}
								handleClose={handleClose}
								onChangeStatus={onChangeStatus}
								isActivityFormOpen={isActivityFormOpen}
								closeActivityForm={handleActivityFormClose}
							/>
						</TabWrapper>
					</TabPanel>
					<TabPanel value={activeTab} index={ProjectFromTabId.USER_ACCESS}>
						<TabWrapper height={tabWrapperHeight}>
							<UserAccessTab projectData={projectData!} />
						</TabWrapper>
					</TabPanel>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
