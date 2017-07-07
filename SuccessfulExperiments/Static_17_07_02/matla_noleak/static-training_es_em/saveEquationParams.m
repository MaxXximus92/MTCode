 function saveEquationParams(equationParams,path)
 if(exist(path)==0)
     a=equationParams(:,1);
     b=equationParams(:,2);
     d=equationParams(:,3);
     save(path,'a','b','d');
 else
     msgID = 'saveEquationParams:FileAllreadyExists';
     msg = ['File allready exisits:' path];
     throw(MException(msgID,msg));
 end
 
 end

