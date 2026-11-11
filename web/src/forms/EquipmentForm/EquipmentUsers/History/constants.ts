export const APPROVE_MESSAGE = 'Are you sure you want to clear user details?';
export const HISTORY_UPDATING_POPUP_TITLE = 'History updating';

export const COLUMNS_WIDTH = {
	userName: 130,
	departmentId: 130,
	issueDate: 115,
	returnDate: 115,
	assignmentApprover: 155,
};

export const COLUMNS_AMOUNT = Object.values(COLUMNS_WIDTH).length;

export const HISTORY_GRID_CONTAINER_MAX_WIDTH = 536;
export const ROW_HEIGHT = 44;

export const MOCK_ROW = {
	id: 0,
	userName: '',
	departmentId: '',
	issueDate: '',
	returnDate: '',
	assignmentApprover: '',
	showSkeleton: true,
};

export const HISTORY_COLUMNS = {
	userName: {
		key: 'userName',
		name: 'User',
		width: COLUMNS_WIDTH.userName,
	},
	departmentId: {
		key: 'departmentId',
		name: 'Department',
		width: COLUMNS_WIDTH.departmentId,
	},
	issueDate: {
		key: 'issueDate',
		name: 'Issue date',
		width: COLUMNS_WIDTH.issueDate,
	},
	returnDate: {
		key: 'returnDate',
		name: 'Return date',
		width: COLUMNS_WIDTH.returnDate,
	},
	assignmentApprover: {
		key: 'assignmentApprover',
		name: 'Assignment approver',
		width: COLUMNS_WIDTH.assignmentApprover,
	},
};
