import { Dispatch, SetStateAction } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWithColorCircleProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { PERIOD } from '@/constants/date';
import { DepartmentType } from '@/types/configuration';

export interface WorkLoad {
	actualFte: number;
	plannedFte: number;
	trackedBillableInHours: number;
	requiredHours: number;
	plannedWorkloadInHours?: number;
}

interface ProjectDetail extends WorkLoad {
	projectId: number;
}

export interface HoursDetail extends WorkLoad {
	monthOfYear: number;
	projectDetails: ProjectDetail[];
	hasActivity: boolean;
	isInputActive?: boolean;
}

interface Client {
	clientId: number;
	name: string;
	color: string;
}

export interface EmployeeProjects {
	projectId: number;
	isActiveAssignement: boolean;
	isTimeOnProjectBillable: boolean;
	isFutureActiveAssignment: boolean;
}

interface Employee {
	userId: number;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	comments: string | null;
	departmentId: string;
	needProject: boolean;
	plan: string | null;
	projects: EmployeeProjects[];
	specializationName: string | null;
	hourDetails: HoursDetail[];
	color: string | null;
	specializationSeniority: string;
	resourceGroupName: string | null;
	resourceGroupId: number | null;
	isActive: boolean;
	canUserInfoBeRetrievedByCurrentUser: boolean;
}

export interface EmployeeInfo {
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	specializationName: string | null;
	specializationSeniority: string;
	isActive: boolean;
}

export enum KpiColors {
	Red = 'Red',
	Orange = 'Orange',
	Green = 'Green',
}

export interface TotalMonth {
	monthOfYear: number;
	totalActualFte: number;
	totalFactBillableInHours: number;
	totalKpi: number;
	totalPlannedFte: number;
	totalPlannedWorkloadInHours: number;
	totalTimePerUsers: number;
	usersCount: number;
	color: KpiColors;
}

export interface PayloadResponse {
	clients: Client[];
	projects: PayloadProject[];
	users: Employee[];
	year: number;
	usersTotal: number;
}

interface PayloadProject {
	projectId: number;
	clientId: number;
	name: string;
}
export interface PayloadProjectWithClientName {
	projectId: number;
	clientId: number;
	color: string;
	name: string;
	clientName: string;
	isActiveAssigment: boolean;
	isTimeOnProjectBillable: boolean;
	isFutureActiveAssignment: boolean;
}

type PayloadMonth = HoursDetail | TotalMonth;

export interface PayloadGridRow {
	expanded?: boolean;
	jan: PayloadMonth;
	feb: PayloadMonth;
	mar: PayloadMonth;
	apr: PayloadMonth;
	may: PayloadMonth;
	jun: PayloadMonth;
	jul: PayloadMonth;
	aug: PayloadMonth;
	sep: PayloadMonth;
	oct: PayloadMonth;
	nov: PayloadMonth;
	dec: PayloadMonth;
	department: string;
	employee: EmployeeInfo;
	comments: string | null;
	needProject: boolean;
	projects: PayloadProjectWithClientName[];
	plan: string | null;
	year: number;
	handleEditClick?: (itemId?: string | number) => () => void;
	id: number;
	group: string | null;
	parentId?: number;
	projectItemsRowHeight?: number;
	color: string;
	isShowSkeleton?: boolean;
	disabled: boolean;
	isLastRow: boolean;
}

interface PayloadColumnsPropsOptions {
	users: string[];
	groups: string[];
	projects: string[];
	departments: SelectionModalOption[];
	displayDepartmentColumn: boolean;
	displayGroupColumn: boolean;
}
interface PayloadColumnsPropsFilters {
	idFilter: string;
	departmentFilter: string[];
	employeeFilter: string;
	commentFilter: string;
	projectFilter: string;
	npFilter: string[];
	planFilter: string;
	groupFilter: string;
}
interface PayloadColumnsPropsSetters {
	setIdFilter: Dispatch<SetStateAction<string>>;
	setDepartmentFilter: Dispatch<SetStateAction<string[]>>;
	setEmployeeFilter: Dispatch<SetStateAction<string>>;
	setCommentFilter: Dispatch<SetStateAction<string>>;
	setNpFilter: Dispatch<SetStateAction<string[]>>;
	setProjectFilter: Dispatch<SetStateAction<string>>;
	setPlanFilter: Dispatch<SetStateAction<string>>;
	setGroupFilter: Dispatch<SetStateAction<string>>;
}
export interface PayloadColumnsProps {
	filters: PayloadColumnsPropsFilters;
	setters: PayloadColumnsPropsSetters;
	options: PayloadColumnsPropsOptions;
}

export interface CellWrapperProps {
	childWidth?: string;
	childHeight?: string;
	maxVerticalLineCount?: number | null;
	isExpanded?: boolean;
	shouldPaintCell?: boolean;
}

export interface MonthCellProps {
	isCurrentMonth: boolean;
}
export interface TotalProps {
	isTotal?: boolean;
	disabled?: boolean;
}

export interface ProjectsGeneralFieldWrapperProps {
	isFirstRow?: boolean;
}

export interface EmployeeFieldProps {
	isSpecializationField?: boolean;
}

export interface ProjectsGeneralFieldsProps {
	newHeight?: number;
}

export interface BodyFilters {
	needProject: boolean | null;
	comment: string | null;
	plan: string | null;
	projectName: string | null;
	userId: string | null;
	userFullName: string | null;
	departmentIds: string[];
	orderingField: string | null;
	isDescendingOrdering: boolean;
	specializationName: string | null;
	resourceGroupName: string | null;
}

export interface ConvertResponseRowProps {
	handleEdit: VoidFunction;
	departmentsData: Array<DepartmentType>;
	responseData?: PayloadResponse;
	page: number;
}

type MonthType = (typeof PERIOD)[number];

export type TotalRow = Record<Lowercase<MonthType>, TotalMonth>;

interface DefaultCellRendererOptionsProps {
	withCircle?: boolean;
	isLogicCell?: boolean;
	withTooltip?: boolean;
	isResizable?: boolean;
	isDepartmentCell?: boolean;
}

export interface DefaultCellRendererProps<T extends CellWithColorCircleProps> {
	props: RenderCellProps<T, unknown>;
	options: DefaultCellRendererOptionsProps;
}

export interface UpdateEmployeeColorRequestBody {
	avatarColor: string;
}

export interface ResizableCellRendererProps {
	projectItemsRowHeight?: number;
}
