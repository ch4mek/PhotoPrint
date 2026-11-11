import {
	User,
	UserClaimsGroup,
} from '@/pages/Settings/components/ClaimsTabs/types';
import { UserClaimsSubmitParams } from '@/queries/types';

export interface UserClaimsFormProps {
	groupClaims: UserClaimsGroup[] | undefined;
	user: User;
	isLoading: boolean;
	onSubmit: (values: UserClaimsSubmitParams) => Promise<object | undefined>;
	handleClose: () => void;
}
