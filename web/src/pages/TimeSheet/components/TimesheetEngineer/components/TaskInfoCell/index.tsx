import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { ThemeMode } from '@mms/mms-ui-library/theme';
import {
	ChangeEvent,
	FocusEvent,
	KeyboardEvent,
	MouseEvent,
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';
import { RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';
import { ValidationError } from 'yup';

import { TimeSheetContext } from '../..';
import { ADD_ROW_ID } from '../../constants';
import { TaskInfoContainer } from '../../styles';
import { getTimesheetRows } from '../../timesheetRowsStorage';
import { ProjectOption, TimeSheetRow } from '../../types';
import { taskNameValidationSchema } from '../../validationSchema';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { DateFormat } from '@/constants/date';
import {
	ERROR_MESSAGE_UPDATE_PROJECT,
	NotificationType,
} from '@/constants/notifications';
import { employees, managerRoles } from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { TimesheetStatusContext } from '@/pages/TimeSheet/context/timesheetStatusContext';

import { TaskNameInput } from './TaskNameInput';

const getAutoselectKey = (option: ProjectOption) => option.id;

const { gridTooltip } = zIndexes;

export function TaskInfoCell(props: RenderCellProps<TimeSheetRow, unknown>) {
	const { rowIdx, row } = props;
	const { t: tCommon } = useTranslation('common');
	const { t: tTimesheet } = useTranslation('timesheet');
	const theme = useTheme();
	const {
		projectName,
		projectId,
		taskName,
		previousTaskName,
		timeMap,
		isValidTaskName,
		isTaskInfoEditable,
		rowId,
		entireRowHoverMessage,
		userHasAccessToProject,
		isProjectActive,
		isEditingForbiddenForPeriod,
		areTaskInfoFieldsTouched,
		isMobileView,
	} = row;

	const { role } = useContext(TimesheetStatusContext);

	const [taskNameError, setTaskNameError] = useState<string | undefined>(
		undefined
	);
	const [projectError, setProjectError] = useState<string | undefined>(
		undefined
	);
	const autoSelectWrapperRef = useRef<HTMLDivElement | null>(null);

	const [isOpenDropDown, setIsOpenDropDown] = useState(false);

	const timeTrackItem = Array.from(timeMap.values()).find(
		(item) =>
			item !== undefined &&
			item.trackId !== 0 &&
			(item.factTrackedInMinutes !== 0 || !!item.billableTrackedInMinutes)
	);

	const ifTimeTracked = !!timeTrackItem;

	useEffect(() => {
		if (projectId) {
			setProjectError(undefined);

			return;
		}

		if (!areTaskInfoFieldsTouched) {
			return;
		}

		if (taskName || !ifTimeTracked) {
			setProjectError(tCommon('tooltips.selectProject'));
		}
	}, [taskName, projectId, ifTimeTracked, areTaskInfoFieldsTouched]);

	const {
		toast,
		onTaskNameUpdate,
		period,
		isEngineer,
		columns,
		projectOptions,
		onParentRowChange,
		userId,
		isDisabledTimesheet,
		onTaskInfoChange,
		displayTooltips,
		isPMOEmployee,
		blockedDays,
	} = useContext(TimeSheetContext);

	const isTaskCellBlocked = useMemo(() => {
		if (blockedDays.length === 0 || managerRoles.includes(role)) {
			return false;
		}

		const isBlocked = Boolean(
			blockedDays.find((day) => {
				const dayjsDay = dayjs(day, DateFormat.YYYY_MM_DD).format(
					DateFormat.DD_MM_YYYY
				);

				return timeMap.has(dayjsDay);
			})
		);

		return (
			projectName && employees.includes(role) && !isPMOEmployee && isBlocked
		);
	}, [blockedDays, timeMap]);

	const inputsHoverMessage = useMemo(() => {
		if (entireRowHoverMessage) {
			return entireRowHoverMessage;
		}

		if (isTaskCellBlocked) {
			return tCommon('tooltips.forbiddenToEdit');
		}

		if (isEngineer || isPMOEmployee) {
			if (!userHasAccessToProject && isProjectActive) {
				return tCommon('tooltips.noAccess');
			}

			if (isEditingForbiddenForPeriod) {
				return tCommon('tooltips.forbiddenToEdit');
			}
		}

		if (!isProjectActive) {
			return tCommon('tooltips.inactiveProject');
		}

		if (!isTaskInfoEditable && !userHasAccessToProject) {
			return tCommon('tooltips.noAccess');
		}

		return undefined;
	}, [
		entireRowHoverMessage,
		isTaskInfoEditable,
		userHasAccessToProject,
		isEditingForbiddenForPeriod,
		isProjectActive,
		isPMOEmployee,
		isTaskCellBlocked,
	]);

	const isTaskNameExists = useCallback(
		(newTaskName: string, taskProjectId: number) => {
			if (!userId) {
				return true;
			}

			const timesheetRows = getTimesheetRows(userId);

			// eslint-disable-next-line usememo-recommendations/detect-heavy-operations
			return timesheetRows
				?.filter(
					({ projectId: rowProjectId, rowId: rowRowId }) =>
						rowId !== rowRowId &&
						rowProjectId === taskProjectId &&
						rowRowId !== ADD_ROW_ID
				)
				.some(
					({ taskName: rowTaskName }) =>
						rowTaskName.trim().toLowerCase() ===
						newTaskName.trim().toLowerCase()
				);
		},
		[rowId, userId]
	);

	const handleTaskNameIsNotUnique = useCallback(
		(highlightRow: boolean, returnPreviousTaskName: boolean) => {
			if (highlightRow) {
				onParentRowChange(rowIdx, rowId, {
					isValidTaskName: false,
					areTimeCellsEditable: false,
				});
			}

			if (returnPreviousTaskName) {
				onParentRowChange(rowIdx, rowId, {
					taskName: previousTaskName,
				});
			}

			toast.open(
				createToast(
					NotificationType.ERROR,
					tTimesheet('notifications.taskNameAlreadyExists')
				)
			);
		},
		[rowIdx, rowId, previousTaskName]
	);

	const handleTaskNameIsUnique = useCallback(() => {
		onParentRowChange(rowIdx, rowId, {
			isValidTaskName: true,
			areTimeCellsEditable: true,
		});
	}, [rowId, rowIdx]);

	const handleRowHasPrevTaskInfo = (
		prevInfoRow: TimeSheetRow
	): TimeSheetRow => {
		const {
			isValidTaskName: prevIsValidTaskName,
			errorMessage,
			areTimeCellsEditable: prevAreTimeCellsEditable,
			isTaskInfoEditable: prevIsTaskInfoEditable,
		} = prevInfoRow;

		if (
			prevIsTaskInfoEditable &&
			!prevIsValidTaskName &&
			!errorMessage &&
			!prevAreTimeCellsEditable
		) {
			return {
				...prevInfoRow,
				isValidTaskName: true,
				areTimeCellsEditable: true,
			};
		}

		return prevInfoRow;
	};

	const handleChangeProject = (value: ProjectOption | null) => {
		if (ifTimeTracked) {
			toast.open(
				createToast(NotificationType.ERROR, ERROR_MESSAGE_UPDATE_PROJECT)
			);
		} else if (value) {
			const {
				id,
				name,
				isCurrentUserAbleToSeeBillableTrackedTime,
				isBillable,
				type,
				disabledDates,
				billabilityDays,
			} = value;
			const newProjectId = +id;
			onParentRowChange(rowIdx, rowId, {
				projectName: name,
				projectId: newProjectId,
				isDoubleTimeCell:
					!isEngineer || isCurrentUserAbleToSeeBillableTrackedTime,
				isProjectBillable: isBillable,
				isProjectPrivate: type === tCommon('projectTypes.private'),
				areTaskInfoFieldsTouched: true,
				disabledDates,
				billabilityDays,
			});

			onTaskInfoChange(
				rowIdx,
				rowId,
				taskName,
				projectId,
				taskName,
				newProjectId,
				handleRowHasPrevTaskInfo
			);

			if (isTaskNameExists(taskName, newProjectId)) {
				handleTaskNameIsNotUnique(true, false);
			} else {
				handleTaskNameIsUnique();
			}
		} else {
			setProjectError(tCommon('tooltips.selectProject'));
			onParentRowChange(rowIdx, rowId, {
				projectName: undefined,
				projectId: 0,
				isProjectPrivate: false,
				areTaskInfoFieldsTouched: true,
			});
		}
	};

	const isEditForbidden = useMemo(
		() =>
			!isProjectActive ||
			isTaskCellBlocked ||
			isDisabledTimesheet ||
			(!isTaskInfoEditable && employees.includes(role)),
		[
			isEngineer,
			columns,
			isTaskCellBlocked,
			isDisabledTimesheet,
			isTaskInfoEditable,
			isProjectActive,
		]
	);

	const projectValue: ProjectOption | undefined = useMemo(() => {
		const taskProject = projectOptions?.find(
			(project) => project.id === projectId
		);

		if (taskProject) {
			return taskProject;
		}

		if (!projectId) {
			return undefined;
		}

		return {
			id: projectId,
			name: projectName,
			type: tCommon('projectTypes.private'),
			client: '',
			isBillable: false,
			isCurrentUserAbleToSeeBillableTrackedTime: false,
			disabledDates: [],
			billabilityDays: [],
		};
	}, [projectOptions, projectId, tCommon]);

	const handleErrorChange = useCallback(
		(errorMessage: undefined | string) => {
			setTaskNameError(errorMessage);
			onParentRowChange(rowIdx, rowId, { errorMessage });
		},
		[rowIdx]
	);

	const stylesConfig = useMemo(
		() => ({
			label: {
				background: theme.palette['primary-1'],
			},
			errorText: {
				wrapper: {
					background: theme.palette['primary-1'],
				},
			},
		}),
		[theme]
	);

	const hasTaskNameError = Boolean(taskNameError) || !isValidTaskName;

	const handleBlur = useCallback(
		(event: FocusEvent<HTMLInputElement>) => {
			if (taskNameError || !userId || isEditForbidden) {
				return;
			}

			if (!areTaskInfoFieldsTouched) {
				onParentRowChange(rowIdx, rowId, {
					areTaskInfoFieldsTouched: true,
				});
			}

			const newTaskName = event.target.value;
			const id = timeTrackItem?.trackId.toString();

			if (isTaskNameExists(newTaskName, projectId)) {
				handleTaskNameIsNotUnique(!ifTimeTracked, ifTimeTracked);

				return;
			}

			handleTaskNameIsUnique();

			if (previousTaskName === newTaskName) {
				return;
			}

			if (!id) {
				onParentRowChange(rowIdx, rowId, {
					taskName: newTaskName,
					previousTaskName: newTaskName,
				});

				return;
			}

			if (
				!ifTimeTracked ||
				!period.length ||
				!period[0] ||
				!period[period.length - 1]
			) {
				return;
			}

			onTaskNameUpdate(
				{
					projectId,
					newTaskName,
					oldTaskName: previousTaskName,
					startDate: period[0]!,
					endDate: period[period.length - 1]!,
				},
				{
					onSuccess: () => {
						onParentRowChange(rowIdx, rowId, {
							taskName: newTaskName,
							previousTaskName: newTaskName,
						});
					},
					onError: () => {
						onParentRowChange(rowIdx, rowId, { taskName: previousTaskName });
					},
				}
			);
		},
		[
			rowId,
			timeTrackItem,
			taskNameError,
			isEditForbidden,
			ifTimeTracked,
			previousTaskName,
			areTaskInfoFieldsTouched,
			isTaskNameExists,
			projectId,
			handleTaskNameIsNotUnique,
			handleTaskNameIsUnique,
			userId,
		]
	);

	const validateTaskName = useCallback(
		(newTaskName: string) => {
			taskNameValidationSchema()
				.validate(newTaskName)
				.then(() => {
					handleErrorChange(undefined);
				})
				.catch((validationError) => {
					if (
						validationError instanceof ValidationError &&
						validationError.inner &&
						validationError.value
					) {
						handleErrorChange(validationError.message);
					} else {
						handleErrorChange(undefined);
					}
				});
		},
		[handleErrorChange]
	);

	const handleOnChange = useCallback(
		(event: ChangeEvent<HTMLInputElement>) => {
			const newTaskName = event.target.value;

			if (!areTaskInfoFieldsTouched) {
				onParentRowChange(rowIdx, rowId, {
					areTaskInfoFieldsTouched: true,
				});
			}

			if (isTaskNameExists(newTaskName, projectId)) {
				onParentRowChange(rowIdx, rowId, {
					isValidTaskName: false,
					areTimeCellsEditable: false,
				});
			} else {
				handleTaskNameIsUnique();
			}

			onTaskInfoChange(
				rowIdx,
				rowId,
				taskName,
				projectId,
				newTaskName,
				projectId,
				handleRowHasPrevTaskInfo
			);

			validateTaskName(newTaskName);
		},
		[
			rowIdx,
			rowId,
			projectId,
			taskName,
			areTaskInfoFieldsTouched,
			isTaskNameExists,
			handleTaskNameIsUnique,
		]
	);

	const handleClickCapture = useCallback(
		(e: MouseEvent<HTMLInputElement>) => e.stopPropagation(),
		[]
	);

	const handleClick = useCallback(
		(e: MouseEvent<HTMLInputElement>) => e.stopPropagation(),
		[]
	);

	const handleKeyDown = (e: KeyboardEvent) => e.stopPropagation();

	return (
		<TaskInfoContainer
			isMobileView={isMobileView}
			onClick={handleClick}
			className="task-info-container"
		>
			<Tooltip
				text={inputsHoverMessage}
				open={displayTooltips && Boolean(inputsHoverMessage)}
				position="bottom"
				zIndex={gridTooltip}
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			>
				<div>
					<div ref={autoSelectWrapperRef}>
						<Autoselect
							label={tCommon('labels.project')}
							value={projectValue || null}
							valueField="name"
							subValueField="client"
							groupByField="type"
							options={projectOptions || []}
							onChange={handleChangeProject}
							disabled={isEditForbidden}
							error={Boolean(projectError)}
							errorText={projectError}
							showToolTipIfCut={!inputsHoverMessage}
							width={200}
							isDropDownOpenOuter={isOpenDropDown}
							setIsDropDownOpenOuter={setIsOpenDropDown}
							getKey={getAutoselectKey}
							onKeyDown={handleKeyDown}
							stylesConfig={stylesConfig}
							labelAndErrorBgColor={
								theme.mode === ThemeMode.DARK
									? theme.palette['primary-1']
									: theme.palette['exc-popups-primary-1']
							}
						/>
					</div>
				</div>
			</Tooltip>
			<Tooltip
				text={inputsHoverMessage}
				open={displayTooltips && Boolean(inputsHoverMessage)}
				position="bottom"
				zIndex={gridTooltip}
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			>
				<div>
					<TaskNameInput
						autoComplete="off"
						id={String(row.projectId)}
						onChange={handleOnChange}
						label={tCommon('labels.task')}
						size={isMobileView ? 'small' : 'medium'}
						onBlur={handleBlur}
						value={taskName}
						stylesConfig={stylesConfig}
						disabled={isEditForbidden}
						onClickCapture={handleClickCapture}
						error={hasTaskNameError}
						placeholder={tTimesheet('formatPlaceholder')}
						showToolTipIfCut={!inputsHoverMessage}
						errorText={
							!isValidTaskName
								? tTimesheet('taskNameAlreadyUsed')
								: taskNameError
						}
						onKeyDown={handleKeyDown}
					/>
				</div>
			</Tooltip>
		</TaskInfoContainer>
	);
}
