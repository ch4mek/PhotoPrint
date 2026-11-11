import { defaultIdColumnWidth } from '@/components/Grid/constants';
import { fromToDateObject } from '@/constants/defaultValues';

export const DEFAULT_REQUESTS_FILTERS = {
	id: '',
	userId: '',
	userFullName: '',
	userPosition: [],
	inquiryTemplateName: '',
	userDepartment: [],
	dateFrom: fromToDateObject,
	dateTo: fromToDateObject,
	status: [],
	createdAtUtc: fromToDateObject,
	notes: '',
};

export const RequestsColumnsWidths = {
	id: defaultIdColumnWidth,
	userFullName: {
		width: 200,
		maxWidth: 350,
	},
	userPosition: {
		width: 164,
		fixed: true,
	},
	userDepartment: {
		width: 164,
		fixed: true,
	},
	inquiryTemplateName: {
		width: 172,
		fixed: true,
	},
	quantity: {
		width: 88,
		fixed: true,
	},
	status: {
		width: 318,
		fixed: true,
	},
	dateFrom: {
		width: 108,
		fixed: true,
	},
	dateTo: {
		width: 100,
		fixed: true,
	},
	createdAtUtc: {
		width: 100,
		fixed: true,
	},
	notes: 260,
};

export const REQUESTS_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
	},
	user: {
		key: 'userFullName',
		name: 'User',
	},
	department: {
		key: 'userDepartmentId',
		name: 'Department',
	},
	position: {
		key: 'userPosition',
		name: 'Position',
	},
	inquiryTemplateName: {
		key: 'inquiryTemplateName',
		name: 'Request type',
	},
	dateFrom: {
		key: 'dateFrom',
		name: 'Date from',
	},
	dateTo: {
		key: 'dateTo',
		name: 'Date to',
	},
	quantity: {
		key: 'quantity',
		name: 'Quantity',
	},

	status: {
		key: 'status',
		name: 'Status',
	},
	created: {
		key: 'createdAtUtc',
		name: 'Created',
	},
	notes: {
		key: 'notes',
		name: 'Notes',
	},
};

export const columnsToReverseOrder = [REQUESTS_COLUMNS.position.key];
export const extraOrderByKeys = {
	statusOrderWeight: 'statusOrderWeight',
	userPositionOrderWeight: 'userPositionOrderWeight',
	userFirstNameRu: 'userFirstNameRu',
	userLastNameRu: 'userLastNameRu',
};
