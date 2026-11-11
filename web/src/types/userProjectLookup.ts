export interface UserProjectLookup {
	id: number;
	projectId: number;
	projectName: string;
	clientName: string;
	isActive: boolean;
	isTimeOnProjectBillable: boolean;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
}
