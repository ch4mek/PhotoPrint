import { Roles } from '@/constants/roles';
import { UserClaims } from '@/types/claims';

export interface ProtectedViewProps {
	roles: Array<Roles>;
	slug?: string;
	claims?: Array<UserClaims>;
}
