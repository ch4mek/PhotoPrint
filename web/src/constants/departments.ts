import { Roles } from './roles';

export enum Departments {
	Js = 'Js',
	Net = 'Net',
	Java = 'Java',
	Python = 'Python',
	QA = 'QA',
	HR = 'HR',
	Recruitment = 'Recruitment',
	Marketing = 'Marketing',
	PMO = 'PMO',
	Sales = 'Sales',
	BA = 'BA',
	Mobile = 'Mobile',
	PHP = 'PHP',
	DevOps = 'DevOps',
	Administration = 'Administration',
	Design = 'Design',
	Ruby = 'Ruby',
	SysAdmin = 'SysAdmin',
	OneC = '1C',
	LD = 'LD',
	SAP = 'SAP',
	AIML = 'AIML',
}

export const isPMO = (department?: string) => {
	if (!department) {
		return false;
	}

	return department === Departments.PMO;
};

export const isHeadOfPMO = (department: string, role: Roles) =>
	isPMO(department) && role === Roles.HD;

export const isSaleManager = (department: string) =>
	department === Departments.Sales;

export const departmentIdsWithCertificates: string[] = [
	Departments.Js,
	Departments.Net,
	Departments.Java,
	Departments.Python,
	Departments.QA,
	Departments.HR,
	Departments.Marketing,
	Departments.PMO,
	Departments.Sales,
	Departments.BA,
	Departments.Mobile,
	Departments.DevOps,
	Departments.Design,
];

export const techDepartments: string[] = [
	Departments.Js,
	Departments.Net,
	Departments.Java,
	Departments.Python,
	Departments.QA,
	Departments.Mobile,
	Departments.PHP,
	Departments.DevOps,
	Departments.Ruby,
];
