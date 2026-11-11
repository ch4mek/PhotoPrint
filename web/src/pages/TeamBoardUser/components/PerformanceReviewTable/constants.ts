export const PERFORMANCE_REVIEW_COLUMNS_KEYS = {
	performanceReviewDate: 'performanceReviewDate',
	theses: 'theses',
	employeeTheses: 'employeeTheses',
	agreements: 'agreements',
	files: 'files',
	documentLinks: 'documentLinks',
};

const {
	performanceReviewDate,
	theses,
	employeeTheses,
	agreements,
	files,
	documentLinks,
} = PERFORMANCE_REVIEW_COLUMNS_KEYS;

export const PERFORMANCE_REVIEW_COLUMNS_NAMES = {
	[performanceReviewDate]: 'Date',
	[theses]: 'Theses',
	[employeeTheses]: 'Employee’s theses',
	[agreements]: 'Agreements',
	[files]: 'Documents',
	[documentLinks]: 'Doc. links',
};

export const PERFORMANCE_REVIEW_TAB_TABLE_WIDTHS = {
	performanceReviewDate: {
		width: 116,
		fixed: true,
	},
	theses: 300,
	employeeTheses: 300,
	agreements: 300,
	files: {
		width: 180,
		fixed: true,
	},
	documentLinks: {
		width: 84,
		fixed: true,
	},
};

export const PERFORMANCE_EDIT_COLUMN_INDEX = 8;
export const PERFORMANCE_REVIEW_LINE_HEIGHT = 18;
